using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Foundation.Areas.Public.Models;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models.Lookups;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/regions")]
    public class PublicRegionsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PublicRegionsController()
        {
        }
        [ResponseType(typeof(ResponseMsg<IEnumerable<RegionViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            var currentUser = db.Users.Find(User.Identity.GetUserId<long>());

            ResponseMsg<IEnumerable<RegionViewModel>> Msg = new ResponseMsg<IEnumerable<RegionViewModel>>();
            Msg.Content = db.Regions.Where(i => i.IsActive && !i.IsDeleted).Include(m=> m.City).ToList().
                            Select(m => new RegionViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
