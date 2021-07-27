using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models.Lookups;
using Domain.Models;
using Foundation.Areas.Admin.Models;
using System.IO;
using System.IO.Compression;
using Foundation.Areas.Public.Models;
using MobileServices.Core;
using System.Net.Http.Headers;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/responses")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]

    public class ResponsesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ResponsesController()
        {


        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<RecipientResponseViewModel>>))]
        [HttpGet]
        [Route("incoming")]
        public HttpResponseMessage Incoming([FromUri] long enquiryId)
        {
            ResponseMsg<IEnumerable<RecipientResponseViewModel>> Msg = new ResponseMsg<IEnumerable<RecipientResponseViewModel>>();

            Msg.Content = db.Recipients
                        .Include(i => i.PublicUser)
                        .Include(r => r.ItemsResponse)
                        .Include(r => r.ItemsResponse.Select(i => i.DynamicPropertiesResponses))
                        .Where(r => r.Invitation.EnquiryId == enquiryId
                                && !r.IsDraftResponse
                                && !r.IsFailed)
                                .ToList().Select(r => new RecipientResponseViewModel(r, r.ItemsResponse));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<InvitationResponseViewModel>))]
        [HttpGet]
        [Route("outgoing/{invitationId}/{publicUserId}")]
        public HttpResponseMessage OutgoingInvitaionResponse(long invitationId, long publicUserId)
        {
            ResponseMsg<InvitationResponseViewModel> Msg = new ResponseMsg<InvitationResponseViewModel>();
            var recipient = db.Recipients
                .Include(i => i.PublicUser)
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.Items)
                .Include(i => i.Invitation.Enquiry.Items.Select(m => m.DynamicProperties))
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => i.InvitationId == invitationId && i.PublicUserId == publicUserId)
                .FirstOrDefault();

            if (recipient == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg);
            }

            var responses = db.ItemResponses
                .Include(i => i.DynamicPropertiesResponses)
                .Where(i => i.Recipient.InvitationId == invitationId && i.Recipient.PublicUserId == publicUserId)
                .ToList();
            Msg.Content = new InvitationResponseViewModel(recipient.IsDraftResponse,
                responses,
                recipient.Invitation.Enquiry.Items);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }



        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("properties/{invitationId}")]
        public HttpResponseMessage Properties(long invitationId)
        {
            ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>();

            var enquiry = db.Recipients
                .Include(i => i.Invitation.Enquiry)
                .Where(i => i.InvitationId == invitationId)
                .Select(i => i.Invitation.Enquiry)
                .FirstOrDefault();

            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg);
            }

            var list = db.EnquiryItemsDynamicProperties
               .Include(m => m.Property)
               .Include(m => m.Property.PropertyType)
               .Where(i => i.EnquiryId == enquiry.Id && i.IsPublic)
               .ToList();
            foreach (var item in list)
            {
                if (item.Property.IsDeleted)
                {
                    item.Property.Name = item.Property.Name.Replace("[Deleted]", "");
                }
            }
            Msg.Content = list.Select(m => new EnquiryItemsDynamicPropertyViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<RecipientResponseViewModel>>))]
        [HttpGet]
        [Route("attachments")]
        public HttpResponseMessage Attachments([FromUri] string recipients)
        {
            if (String.IsNullOrWhiteSpace(recipients))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            int[] recipientsIds = null;
            try
            {
                recipientsIds = recipients.Split(',').Select(str => Int32.Parse(str)).ToArray();
                if (recipientsIds.Length == 0)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }


            var files = db.RecipientResponseAttachments
                        .Include(i => i.Recipient.PublicUser)
                        .Where(i => !i.IsDeleted
                                && !i.Recipient.IsDraftResponse
                                && !i.Recipient.IsFailed)
                                .ToList()
                                .Join(recipientsIds, f => f.RecipientId, r => r, (f, r) => f)
                                .ToList();
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            byte[] finalFileContent = null;
            using (var memoryStream = new MemoryStream())
            {
                
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var usernames = files.Select(i => i.Recipient.PublicUser.UserName).Distinct();
                    foreach (var username in usernames)
                    {
                        //var userDirectoryEntry = archive.CreateEntry(username + "/");
                        var userfiles = files.Where(i => i.Recipient.PublicUser.UserName == username).ToList();
                        foreach (var file in userfiles)
                        {
                            var fileEntry = archive.CreateEntry(username.Replace("@","_at_") + "/" + file.FileName);
                            using (var fileEntryStream = fileEntry.Open())
                            {
                                var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetResponsessFolder())
                                                    + file.FileName;
                                var fileBytes = File.ReadAllBytes(filePath);
                                fileEntryStream.Write(fileBytes, 0, fileBytes.Length);
                            }
                        }
                    }

                }

                finalFileContent = memoryStream.ToArray();
                
               
            }

            result.Content = new ByteArrayContent(finalFileContent);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "Attachments.zip"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

            return result;


        }

    }
}
