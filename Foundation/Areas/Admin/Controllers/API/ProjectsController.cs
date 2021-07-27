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
    [RoutePrefix("admin/api/projects")]
    [CustomAuthorize(SystemRoles.ViewProjects)]

    public class ProjectsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ProjectsController()
        {
            

        }

       

        [ResponseType(typeof(ResponseMsg<IEnumerable<ProjectViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Search([FromUri]ProjectSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new ProjectSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ProjectViewModel>> Msg = new ResponseMsg<IEnumerable<ProjectViewModel>>();

            Msg.Content = db.Projects
               .Include(m => m.Client)
               .Include(m => m.Client.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ProjectViewModel(m));
            Msg.TotalCount = db.Projects.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ProjectViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<ProjectViewModel> Msg = new ResponseMsg<ProjectViewModel>();
            var item = db.Projects.Include(i => i.Client.PublicUser).FirstOrDefault(i => i.Id==id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            db.Entry(item).Reference(u => u.Client).Load();
            Msg.Content = new ProjectViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

       
       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
