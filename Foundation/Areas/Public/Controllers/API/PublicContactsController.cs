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
    [RoutePrefix("api/contacts")]
    public class PublicContactsController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1; 

        public PublicContactsController()
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
        [ResponseType(typeof(ResponseMsg<IEnumerable<ContactViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage All()
        {
            ResponseMsg<IEnumerable<ContactViewModel>> Msg = new ResponseMsg<IEnumerable<ContactViewModel>>();
            Msg.Content = db.Contacts.Include(i => i.ContactType)
                .Where(i => i.PublicUserId == currentUserId)
                .Include(i => i.PublicUser)
               .ToList().
               Select(m => new ContactViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ContactViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Get([FromUri]ContactSearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new ContactSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ContactViewModel>> Msg = new ResponseMsg<IEnumerable<ContactViewModel>>();

            List<Contact> contacts = db.Contacts.Include(i => i.ContactType)
                .Include(i => i.PublicUser)
                .Where(i => i.PublicUserId == currentUserId)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList();
            List<ContactViewModel> cvmList = new List<ContactViewModel>();
            for (int index=0; index < contacts.Count(); index++)
            {
                List<ContactType> contactTypeList = new List<ContactType>();
                Contact m = contacts[index];
                List<VendorContactCategory> vendors = db.VendorContactCategories.Include(i=>i.ContactType).Where(i => i.ContactId == m.Id && i.IsDeleted == false).ToList();
                vendors.ForEach(v =>
                {
                    contactTypeList.Add(v.ContactType);
                });
                ContactViewModel cvm = new ContactViewModel(m);
                cvm.ContactTypes = contactTypeList.Select(c=>new Models.ViewModels.ContactTypeBriefViewModel(c));
                cvmList.Add(cvm);
            }
            Msg.Content = cvmList.Select(m=>m);

            Msg.TotalCount = db.Contacts.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ContactViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<ContactViewModel> Msg = new ResponseMsg<ContactViewModel>();
            var item = db.Contacts.Include(i => i.PublicUser).Include(i => i.ContactType).FirstOrDefault(i => i.Id == id 
                        && i.PublicUserId != currentUserId);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new ContactViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ContactViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(ContactPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = new Contact();
            model.UpdateModel(itemToSave);
            var contactType = db.ContactTypes.FirstOrDefault(i => i.Id == itemToSave.ContactTypeId
                        && i.PublicUserId == currentUserId);
            if (contactType == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            itemToSave.PublicUserId = currentUserId;

            itemToSave.CreationDate = DateTime.UtcNow;
            itemToSave.CreatorId = currentUserId;
            
            db.Contacts.Add(itemToSave);
            try
            {
                db.SaveChanges();
                
                string[] contactTypesList = model.ContactTypes.Split(',');
                for(int i=0;i<contactTypesList.Length;i++)
                {
                    long contactTypesId = int.Parse(contactTypesList[i]);
                    long contactId = itemToSave.Id;
                    VendorContactCategory vcc = new VendorContactCategory();
                    vcc.ContactId = contactId;
                    vcc.ContactTypeId = contactTypesId;
                    vcc.CreationDate = DateTime.UtcNow;
                    vcc.CreatorId = currentUserId;
                    db.VendorContactCategories.Add(vcc);
                }
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            

            ResponseMsg<ContactViewModel> Msg = new ResponseMsg<ContactViewModel>();
            db.Entry(itemToSave).Reference(i => i.PublicUser).Load();
            Msg.Content = new ContactViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ContactViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, ContactPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var itemToSave = db.Contacts.Include(i => i.PublicUser).FirstOrDefault(i => i.Id==id
                && i.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(itemToSave);
            var contactType = db.ContactTypes.FirstOrDefault(i => i.Id == itemToSave.ContactTypeId
                       && i.PublicUserId == currentUserId);
            if (contactType == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            try
            {
                db.SaveChanges();

                List<VendorContactCategory> originContacts = db.VendorContactCategories.Where(i => i.ContactId == id).ToList();
                db.VendorContactCategories.RemoveRange(originContacts);
                db.SaveChanges();

                string[] contactTypesList = model.ContactTypes.Split(',');
                for (int i = 0; i < contactTypesList.Length; i++)
                {
                    long contactTypesId = int.Parse(contactTypesList[i]);
                    long contactId = itemToSave.Id;
                    VendorContactCategory vcc = new VendorContactCategory();
                    vcc.ContactId = contactId;
                    vcc.ContactTypeId = contactTypesId;
                    vcc.CreationDate = DateTime.UtcNow;
                    vcc.CreatorId = currentUserId;
                    db.VendorContactCategories.Add(vcc);
                }
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<ContactViewModel> Msg = new ResponseMsg<ContactViewModel>();
            Msg.Content = new ContactViewModel(itemToSave);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var itemToSave = db.Contacts.FirstOrDefault(i => i.Id == id
                       && i.PublicUserId == currentUserId);
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

                List<VendorContactCategory> originContacts = db.VendorContactCategories.Where(i => i.ContactId == id).ToList();
                db.VendorContactCategories.RemoveRange(originContacts);
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
