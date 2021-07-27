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
    [RoutePrefix("admin/api/item-types")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]
    public class ItemTypesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ItemTypesController()
        {
            

        }
      
        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemTypeViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]ItemTypeSearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new ItemTypeSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ItemTypeViewModel>> Msg = new ResponseMsg<IEnumerable<ItemTypeViewModel>>();

            Msg.Content = db.ItemTypes
                .Include(i => i.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ItemTypeViewModel(m));
            Msg.TotalCount = db.ItemTypes.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
