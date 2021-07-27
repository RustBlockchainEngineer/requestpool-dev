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
    [RoutePrefix("admin/api/enquiry-types")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]
    public class EnquiryTypesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public EnquiryTypesController()
        {
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryTypeViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]EnquiryTypeSearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new EnquiryTypeSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<EnquiryTypeViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryTypeViewModel>>();

            Msg.Content = db.EnquiryTypes
                .Include(i => i.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new EnquiryTypeViewModel(m));
            Msg.TotalCount = db.EnquiryTypes.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
