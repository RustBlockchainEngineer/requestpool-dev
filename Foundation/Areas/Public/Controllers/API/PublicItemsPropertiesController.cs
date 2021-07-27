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

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/items-properties")]
    public class PublicItemsPropertiesController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public PublicItemsPropertiesController()
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
        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage All()
        {
            ResponseMsg<IEnumerable<ItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<ItemsDynamicPropertyViewModel>>();
            Msg.Content = db.ItemsDynamicProperties

                .Where(i => i.PublicUserId == currentUserId)
                .Include(i => i.PublicUser)
                .Include(m => m.PropertyType)
               .ToList().
               Select(m => new ItemsDynamicPropertyViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<ItemsDynamicPropertyViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<ItemsDynamicPropertyViewModel> Msg = new ResponseMsg<ItemsDynamicPropertyViewModel>();
            var item = db.ItemsDynamicProperties.Include(i => i.PublicUser).FirstOrDefault(m => m.Id == id && m.PublicUserId == currentUserId);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new ItemsDynamicPropertyViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ItemsDynamicPropertyViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(ItemsDynamicPropertyPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = new ItemsDynamicProperty();
            model.UpdateModel(itemToSave);
            itemToSave.PublicUserId = currentUserId;

            itemToSave.CreationDate = DateTime.UtcNow;
            itemToSave.CreatorId = currentUserId;

            db.ItemsDynamicProperties.Add(itemToSave);
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

            ResponseMsg<ItemsDynamicPropertyViewModel> Msg = new ResponseMsg<ItemsDynamicPropertyViewModel>();
            db.Entry(itemToSave).Reference(m => m.PropertyType).Load();
            db.Entry(itemToSave).Reference(m => m.PublicUser).Load();            
            Msg.Content = new ItemsDynamicPropertyViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ItemsDynamicPropertyViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, ItemsDynamicPropertyPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = db.ItemsDynamicProperties.Include(i => i.PublicUser).FirstOrDefault(m => m.Id == id && m.PublicUserId == currentUserId);
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
            ResponseMsg<ItemsDynamicPropertyViewModel> Msg = new ResponseMsg<ItemsDynamicPropertyViewModel>();
            db.Entry(itemToSave).Reference(m => m.PropertyType).Load();
            Msg.Content = new ItemsDynamicPropertyViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var itemToSave = db.ItemsDynamicProperties.FirstOrDefault(m => m.Id == id && m.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            itemToSave.IsDeleted = true;
            itemToSave.Name = "[Deleted]" + itemToSave.Name;

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
