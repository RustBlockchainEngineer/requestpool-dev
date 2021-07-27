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
    [RoutePrefix("api/enquiries-attachments")]
    public class PublicEnquiryAttachmentsController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicEnquiryAttachmentsController()
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

        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryAttachmentViewModel>>))]
        [HttpGet]
        [Route("{enquiryId}")]
        public async Task<HttpResponseMessage> Get(long enquiryId)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<IEnumerable<EnquiryAttachmentViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryAttachmentViewModel>>();
            Msg.Content = db.EnquiryAttachments
                .Where(m => m.EnquiryId == enquiryId && m.Enquiry.PublicUserId ==currentUserId && !m.IsDeleted)
                .ToList()
                .Select(m => new EnquiryAttachmentViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

       

        [ResponseType(typeof(ResponseMsg<EnquiryAttachmentViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(EnquiryAttachmentPostModel model)
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
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }


            var enquiry = db.Enquiries.FirstOrDefault(i => i.Id == model.EnquiryId && i.PublicUserId == currentUserId);
            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = new EnquiryAttachment();
            model.UpdateModel(itemToSave);
            itemToSave.CreationDate = DateTime.UtcNow;
            itemToSave.CreatorId = currentUserId;
            String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetEnquiriesFolder());
            String FileName = model.EnquiryId + "-" + DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ffffff") + Path.GetExtension(model.OriginalFileName);
            if (!UploadHelper.IsValidFile( FileName))
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
            db.EnquiryAttachments.Add(itemToSave);
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

            ResponseMsg<EnquiryAttachmentViewModel> Msg = new ResponseMsg<EnquiryAttachmentViewModel>();
            db.Entry(itemToSave).Reference(u => u.Creator).Load();
            Msg.Content = new EnquiryAttachmentViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<EnquiryAttachmentViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(int id, EnquiryAttachmentPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = db.EnquiryAttachments.FirstOrDefault(i=>i.Id == id && i.Enquiry.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(itemToSave);
            db.Entry<EnquiryAttachment>(itemToSave).State = EntityState.Modified;
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

            ResponseMsg<EnquiryAttachmentViewModel> Msg = new ResponseMsg<EnquiryAttachmentViewModel>();
            Msg.Content = new EnquiryAttachmentViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(int id)
        {
            var itemToSave = db.EnquiryAttachments.FirstOrDefault(i => i.Id == id && i.Enquiry.PublicUserId == currentUserId);

            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetEnquiriesFolder());
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

            db.Entry<EnquiryAttachment>(itemToSave).State = EntityState.Modified;
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
