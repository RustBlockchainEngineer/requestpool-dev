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
using Foundation.Areas.Public.Models;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/property-types")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]

    public class PropertyTypesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PropertyTypesController()
        {
            

        }
       
        [ResponseType(typeof(ResponseMsg<IEnumerable<PropertyTypeBriefViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            ResponseMsg<IEnumerable<PropertyTypeBriefViewModel>> Msg = new ResponseMsg<IEnumerable<PropertyTypeBriefViewModel>>();
            Msg.Content = db.PropertyTypes.ToList().
                            Select(m => new PropertyTypeBriefViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
