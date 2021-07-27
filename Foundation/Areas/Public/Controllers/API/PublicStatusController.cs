using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Foundation.Areas.Admin.Models.ViewModels;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models.Lookups;

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/status")]
    public class PublicStatusController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public PublicStatusController()
        {
        }
        [ResponseType(typeof(ResponseMsg<IEnumerable<StatusViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            ResponseMsg<IEnumerable<StatusViewModel>> Msg = new ResponseMsg<IEnumerable<StatusViewModel>>();
            Msg.Content = db.Status.Where(m=> m.IsActive && !m.IsDeleted)
                            .ToList().Select(m => new StatusViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
