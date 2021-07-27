using Foundation.Areas.Admin.Models;
using Foundation.Areas.Admin.Models.ViewModels;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/roles")]
    public class RolesController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [ResponseType(typeof(ResponseMsg<IEnumerable<RoleViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Get()
        {
            ResponseMsg<IEnumerable<RoleViewModel>> Msg = new ResponseMsg<IEnumerable<RoleViewModel>>();
            var roles = db.Roles.Where(r=> r.Name != SystemRoles.System 
                                        && r.Name != SystemRoles.Super
                                        && r.Name != SystemRoles.Admin
                                        && r.Name != SystemRoles.PublicUser).ToList();
            Msg.Content = roles.Select(m => new RoleViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

    }
}
