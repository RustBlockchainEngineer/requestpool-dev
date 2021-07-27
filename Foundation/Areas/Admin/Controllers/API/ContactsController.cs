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
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/contacts")]
    [CustomAuthorize(SystemRoles.ViewContacts)]
    public class ContactsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ContactsController()
        {
            
        }
       
       
        [ResponseType(typeof(ResponseMsg<IEnumerable<ContactViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]ContactSearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new ContactSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ContactViewModel>> Msg = new ResponseMsg<IEnumerable<ContactViewModel>>();

            Msg.Content = db.Contacts.Include(i => i.ContactType)
                .Include(i => i.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ContactViewModel(m));
            Msg.TotalCount = db.Contacts.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ContactViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<ContactViewModel> Msg = new ResponseMsg<ContactViewModel>();
            var item = db.Contacts.Include(i => i.PublicUser).Include(i => i.ContactType).FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new ContactViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
