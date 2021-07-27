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
    [RoutePrefix("admin/api/items")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]
    public class ItemsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ItemsController()
        {
        }
        
        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemViewModel>>))]
        [HttpGet]
        [Route("{enquiryId}")]
        public HttpResponseMessage Get(long enquiryId)
        {
            ResponseMsg<IEnumerable<ItemViewModel>> Msg = new ResponseMsg<IEnumerable<ItemViewModel>>();

            Msg.Content = db.Items
                .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.ItemType)
               .Include(m => m.DynamicProperties)
               .Where(i => !i.IsDeleted && i.EnquiryId == enquiryId)
               .ToList().
               Select(m => new ItemViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);

            /*if (searchModel == null)
                searchModel = new ItemSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ItemViewModel>> Msg = new ResponseMsg<IEnumerable<ItemViewModel>>();

            Msg.Content = db.ItemDynamicProperties
               .Include(i => i.Item)
               .Include(i => i.Item.ItemType)
               .Include(i => i.Item.Enquiry.PublicUser)
               .Where(i => i.Item.EnquiryId == enquiryId)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ItemViewModel(m.Item));
            Msg.TotalCount = db.ItemDynamicProperties
                    .Where(i => i.Item.EnquiryId == enquiryId)
                    .Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
            */
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ItemViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]ItemSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new ItemSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ItemSearchViewModel>> Msg = new ResponseMsg<IEnumerable<ItemSearchViewModel>>();

            Msg.Content = db.ItemDynamicPropertyResponses
              .Include(i => i.ItemResponse.Item)
              .Include(i => i.ItemResponse.Item.ItemType)
              .Include(i => i.ItemResponse.Item.Enquiry.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ItemSearchViewModel(m));
            Msg.TotalCount = db.ItemDynamicPropertyResponses.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>))]
        [HttpGet]
        [Route("included-properties/{enquiryId}")]
        public HttpResponseMessage Properties(long enquiryId)
        {
            ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryItemsDynamicPropertyViewModel>>();
            var list = db.EnquiryItemsDynamicProperties
              .Include(m => m.Property)
              .Include(m => m.Property.PropertyType)
              .Where(i => i.EnquiryId == enquiryId)
              .ToList();

            Msg.Content = list.Select(m => new EnquiryItemsDynamicPropertyViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}


