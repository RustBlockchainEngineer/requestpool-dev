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
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/contact-types")]
    [CustomAuthorize(SystemRoles.ViewContacts)]
    public class ContactTypesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ContactTypesController()
        {

        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<ContactTypeViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]ContactTypeSearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new ContactTypeSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ContactTypeViewModel>> Msg = new ResponseMsg<IEnumerable<ContactTypeViewModel>>();

            Msg.Content = db.ContactTypes
                .Include(i => i.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ContactTypeViewModel(m));
            Msg.TotalCount = db.ContactTypes.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
