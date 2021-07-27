using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Foundation.Areas.Public.Models.ViewModels;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models.Lookups;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/contact-types")]
    public class PublicContactTypesController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicContactTypesController()
        {

        }

        long currentUserId { get
            {
                if(_currentUserId == -1)
                    _currentUserId = User.Identity.GetUserId<long>();
                return _currentUserId;
            }
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ContactTypeViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            

            ResponseMsg<IEnumerable<ContactTypeViewModel>> Msg = new ResponseMsg<IEnumerable<ContactTypeViewModel>>();
            Msg.Content = db.ContactTypes.Where(i => i.PublicUserId == currentUserId)
                .Include(i => i.PublicUser)
                .ToList()
                .Select(m => new ContactTypeViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ContactTypeViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(ContactTypeFormModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = new ContactType();
            model.UpdateModel(itemToSave);
            itemToSave.PublicUserId = currentUserId;


            itemToSave.CreationDate = DateTime.UtcNow;
            itemToSave.CreatorId = currentUserId;
            db.ContactTypes.Add(itemToSave);
            try
            {
                db.SaveChanges();
            }
            catch(Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<ContactTypeViewModel> Msg = new ResponseMsg<ContactTypeViewModel>();
            db.Entry(itemToSave).Reference(i => i.PublicUser).Load();
            Msg.Content = new ContactTypeViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ContactTypeViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, ContactTypeFormModel model)
        {
            
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = db.ContactTypes.Include(i => i.PublicUser).FirstOrDefault(i => i.Id == id && i.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(itemToSave);
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
            ResponseMsg<ContactTypeViewModel> Msg = new ResponseMsg<ContactTypeViewModel>();
            
            Msg.Content = new ContactTypeViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var itemToSave = db.ContactTypes.FirstOrDefault(i => i.Id == id && i.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            itemToSave.IsDeleted = true;
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
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
