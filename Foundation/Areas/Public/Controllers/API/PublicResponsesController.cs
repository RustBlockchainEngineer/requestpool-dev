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
using Foundation.Areas.Public.Models;
using Foundation.Templates;
using System.IO.Compression;
using System.Net.Http.Headers;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/responses")]
    public class PublicResponsesController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicResponsesController()
        {
            

        }
        long currentUserId
        {
            get
            {
                if (_currentUserId == -1)
                    _currentUserId = User.Identity.GetUserId<long>();
                return _currentUserId;
            }
        }
        [ResponseType(typeof(ResponseMsg<IEnumerable<RecipientResponseViewModel>>))]
        [HttpGet]
        [Route("incoming")]
        public HttpResponseMessage Incoming([FromUri]long enquiryId)
        {
            ResponseMsg<IEnumerable<RecipientResponseViewModel>> Msg = new ResponseMsg<IEnumerable<RecipientResponseViewModel>>();

            Msg.Content =  db.Recipients
                        .Include(r => r.ItemsResponse)
                        .Include(r => r.ItemsResponse.Select(i=> i.DynamicPropertiesResponses))
                        .Where(r => r.Invitation.EnquiryId == enquiryId
                                && !r.IsDraftResponse
                                && !r.IsFailed
                                && r.Invitation.Enquiry.PublicUserId == currentUserId)
                                .ToList().Select(r => new RecipientResponseViewModel(r,r.ItemsResponse));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<InvitationResponseViewModel>))]
        [HttpGet]
        [Route("outgoing")]
        public HttpResponseMessage OutgoingInvitaionResponse([FromUri]long invitationId)
        {
            ResponseMsg<InvitationResponseViewModel> Msg = new ResponseMsg<InvitationResponseViewModel>();
            var recipient = db.Recipients
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.Items)
               .Include(i => i.Invitation.Enquiry.Items.Select(m=> m.DynamicProperties))
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => i.InvitationId == invitationId && i.PublicUserId == currentUserId)
                .FirstOrDefault();

            if (recipient == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg);
            }

            var responses = db.ItemResponses
                .Include(i=> i.DynamicPropertiesResponses)
                .Where(i => i.Recipient.InvitationId == invitationId && i.Recipient.PublicUserId == currentUserId)
                .ToList();
            Msg.Content = new InvitationResponseViewModel(recipient.IsDraftResponse, 
                responses, 
                recipient.Invitation.Enquiry.Items);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<InvitationResponseViewModel>))]
        [HttpPost]
        [Route("{invitationId}")]
        public async Task<HttpResponseMessage> PostInvitationResponse(long invitationId, InvitationResponsePostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<InvitationResponseViewModel> Msg = new ResponseMsg<InvitationResponseViewModel>();
            var recipient = db.Recipients
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.Items)
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => i.InvitationId == invitationId
                        && (!i.Invitation.EndDate.HasValue
                                || DbFunctions.DiffDays(DateTime.UtcNow, i.Invitation.EndDate.Value) >= 0)
                                && (!i.Invitation.Enquiry.StatusId.HasValue || i.Invitation.Enquiry.StatusId.Value == (long)SystemStatus.Open)
                        && i.PublicUserId == currentUserId)
                .FirstOrDefault();

            if (recipient == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg);
            }
            if (recipient.IsDraftResponse) {
                recipient.IsDraftResponse = model.IsDraftResponse;
            }

            if (!model.IsDraftResponse || !recipient.IsDraftResponse)
            {
                recipient.ResponseSubmitDate = DateTime.UtcNow;
            }
            else
            {
                recipient.ResponseSubmitDate = null;
            }
            var itemResponsesToDelete = db.ItemResponses.Where(i => i.Recipient.InvitationId == invitationId
                        && i.Recipient.PublicUserId == currentUserId);
            var itemDynamicPropertiesResponsesToDelete = db.ItemDynamicPropertyResponses.Where(i => i.ItemResponse.Recipient.InvitationId == invitationId
                        && i.ItemResponse.Recipient.PublicUserId == currentUserId);
            db.ItemDynamicPropertyResponses.RemoveRange(itemDynamicPropertiesResponsesToDelete);
            db.ItemResponses.RemoveRange(itemResponsesToDelete);
            var enquiryItems = db.Items
               .Include(m => m.ItemType)
               .Include(m => m.DynamicProperties)
               .Where(i => i.EnquiryId == recipient.Invitation.EnquiryId)
               .ToList();
            var properties = db.EnquiryItemsDynamicProperties
               .Include(m => m.Property)
               .Include(m => m.Property.PropertyType)
               .Where(i => i.EnquiryId == recipient.Invitation.EnquiryId && i.IsPublic)
               .ToList();
            foreach (InvitationResponseFormModel i in model.Items)
            {
                var itemToSave = new ItemResponse();
                i.UpdateModel(itemToSave);
                itemToSave.RecipientId = recipient.Id;
                itemToSave.CreatorId = currentUserId;
                Item currentEnquiryItem = null;
                foreach(Item enquiryItem in enquiryItems)
                {
                    if(enquiryItem.Id == i.ItemId)
                    {
                        currentEnquiryItem = enquiryItem;
                        break;
                    }
                }
                if (currentEnquiryItem == null)
                    continue;
                db.ItemResponses.Add(itemToSave);
                itemToSave.DynamicPropertiesResponses = new List<ItemDynamicPropertyResponse>();
                
                foreach (ItemDynamicPropertyResponseFormModel p in i.Properties)
                {
                    bool found = false, isPublic = false, isInfoOnly = false,isApplicable=true,isReadOnly = false;
                    var propertyToSave = db.ItemDynamicPropertyResponses.Create();
                    p.UpdateModel(propertyToSave);
                    propertyToSave.CreatorId = currentUserId;

                    foreach (var property in properties)
                    {
                        if(property.PropertyId == p.PropertyId)
                        {
                            found = true;
                            isInfoOnly = property.IsInfoOnly;
                            isPublic = property.IsPublic;
                            break;
                        }
                    }
                    if (!found || !isPublic || isInfoOnly)
                        continue;
                    foreach (var enquiryItemProperty in currentEnquiryItem.DynamicProperties)
                    {
                        if (enquiryItemProperty.DynamicPropertyId == p.PropertyId)
                        {
                            if(!enquiryItemProperty.IsApplicable || enquiryItemProperty.IsReadOnly)
                            {
                                propertyToSave.Value = enquiryItemProperty.Value;
                            }
                            break;
                        }
                    }
                    itemToSave.DynamicPropertiesResponses.Add(propertyToSave);
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            if (!recipient.IsDraftResponse)
            {
                SendNotification(recipient.PublicUser, recipient.Invitation.Enquiry.PublicUser, 
                    recipient.Invitation.Subject);
            }
            return OutgoingInvitaionResponse(invitationId);
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("properties/{invitationId}")]
        public HttpResponseMessage Properties(long invitationId)
        {
            ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>();

            var enquiry = db.Recipients
                .Include(i => i.Invitation.Enquiry)
                .Where(i => i.InvitationId == invitationId && i.PublicUserId == currentUserId)
                .Select(i=> i.Invitation.Enquiry)
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
                            var fileEntry = archive.CreateEntry(username.Replace("@", "_at_") + "/" + file.FileName);
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

        protected bool SendNotification(PublicUser responder, PublicUser sender, string subject)
        {
            bool result = false;

            string message = TemplatesManager.Render(TemplateKeys.Response,
                new ResponseTemplateModel()
                {
                    Name = sender.Name,
                    Responder = responder.Name,
                    Subject = subject,
                    Url = AppSettings.Server
                });
            if (MailService.Send(sender.Email, Resources.Mail.invitation_response_subject, message))
            {
                result = true;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
            }
            return result;
        }
    }
}
