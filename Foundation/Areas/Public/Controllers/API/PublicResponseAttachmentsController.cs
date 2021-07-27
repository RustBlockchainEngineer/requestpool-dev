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
using System.Data.Common;
using System.Data.SqlClient;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/response-attachments/{invitationId}")]
    public class PublicResponseAttachmentsController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicResponseAttachmentsController()
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

        [ResponseType(typeof(ResponseMsg<IEnumerable<RecipientResponseAttachmentViewModel>>))]
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> Get(long invitationId)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<IEnumerable<RecipientResponseAttachmentViewModel>> Msg = new ResponseMsg<IEnumerable<RecipientResponseAttachmentViewModel>>();
            Msg.Content = db.RecipientResponseAttachments
                .Where(m => m.Recipient.PublicUserId == currentUserId
                        && m.Recipient.InvitationId == invitationId
                        && !m.IsDeleted)
                .ToList()
                .Select(m => new RecipientResponseAttachmentViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }



        [ResponseType(typeof(ResponseMsg<RecipientResponseAttachmentViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(long invitationId, RecipientResponseAttachmentPostModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Content))
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            byte[] file = null;
            try
            {
                file = Convert.FromBase64String(model.Content);
                if ((file.Length / 1024) > AppSettings.MaxUploadSizeInKB)
                {
                    throw new Exception("File exceeds max upload size.");
                }
            }
            catch
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var recipient = db.Recipients.FirstOrDefault(i => i.PublicUserId == currentUserId
                    && i.InvitationId == invitationId
                    && !i.IsDeleted
                    && !i.IsFailed
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted
                    && (!i.Invitation.EndDate.HasValue || DbFunctions.DiffDays(DateTime.Now, i.Invitation.EndDate) > 0));
            if (recipient == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = new RecipientResponseAttachment();
            model.UpdateModel(itemToSave);
            itemToSave.CreationDate = DateTime.UtcNow;
            itemToSave.CreatorId = currentUserId;
            itemToSave.RecipientId = recipient.Id;
            String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetResponsessFolder());
            String FileName = recipient.Id + "-" + DateTime.UtcNow.ToString("h-m-fff") + "-" + itemToSave.OriginalFileName;
            if (!UploadHelper.IsValidFile(FileName))
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            try
            {
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);
                File.WriteAllBytes(DirectoryPath + "/" + FileName, file);
                itemToSave.FileName = FileName;
            }
            catch (Exception ex)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            db.RecipientResponseAttachments.Add(itemToSave);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }

            ResponseMsg<RecipientResponseAttachmentViewModel> Msg = new ResponseMsg<RecipientResponseAttachmentViewModel>();
            db.Entry(itemToSave).Reference(u => u.Creator).Load();
            Msg.Content = new RecipientResponseAttachmentViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<RecipientResponseAttachmentViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long invitationId, int id, RecipientResponseAttachmentPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = db.RecipientResponseAttachments.FirstOrDefault(i => i.Id == id
            && i.Recipient.PublicUserId == currentUserId
            && i.Recipient.InvitationId == invitationId
            && i.Recipient.IsDraftResponse
            && (!i.Recipient.Invitation.EndDate.HasValue || DbFunctions.DiffDays(DateTime.Now, i.Recipient.Invitation.EndDate) > 0));
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(itemToSave);
            db.Entry<RecipientResponseAttachment>(itemToSave).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }

            ResponseMsg<RecipientResponseAttachmentViewModel> Msg = new ResponseMsg<RecipientResponseAttachmentViewModel>();
            Msg.Content = new RecipientResponseAttachmentViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long invitationId, int id)
        {
            var itemToSave = db.RecipientResponseAttachments.FirstOrDefault(i => i.Id == id
              && i.Recipient.PublicUserId == currentUserId
              && i.Recipient.InvitationId == invitationId
              && !i.Recipient.Invitation.IsDeleted
              && !i.Recipient.Invitation.IsDraft
              && !i.Recipient.Invitation.Enquiry.IsDeleted
              && (!i.Recipient.Invitation.EndDate.HasValue || DbFunctions.DiffDays(DateTime.Now, i.Recipient.Invitation.EndDate) > 0));

            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetResponsessFolder());
            try
            {
                File.Move(DirectoryPath + "/" + itemToSave.FileName, DirectoryPath + "/[deleted]-" + itemToSave.FileName);
            }
            catch (Exception ex)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }

            itemToSave.FileName = "[deleted]" + itemToSave.FileName;
            itemToSave.IsDeleted = true;

            db.Entry<RecipientResponseAttachment>(itemToSave).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
