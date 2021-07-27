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
    [RoutePrefix("admin/api/items-properties")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]
    public class ItemsPropertiesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ItemsPropertiesController()
        {
            
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]ItemsDynamicPropertySearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new ItemsDynamicPropertySearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<ItemsDynamicPropertyViewModel>>();

            Msg.Content = db.ItemsDynamicProperties
               .Include(m => m.PropertyType)
               .Include(i => i.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ItemsDynamicPropertyViewModel(m));
            Msg.TotalCount = db.ItemsDynamicProperties.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<ItemsDynamicPropertyViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<ItemsDynamicPropertyViewModel> Msg = new ResponseMsg<ItemsDynamicPropertyViewModel>();
            var item = db.ItemsDynamicProperties.Include(i => i.PublicUser).FirstOrDefault(m => m.Id == id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new ItemsDynamicPropertyViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
