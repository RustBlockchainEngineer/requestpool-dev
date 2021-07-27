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
using Foundation.Areas.Public.Models;
using System.IO;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/clients")]
    [CustomAuthorize(SystemRoles.ViewClients)]
    public class ClientsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ClientsController()
        {
            
        }
        

        [ResponseType(typeof(ResponseMsg<IEnumerable<ClientViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Get([FromUri]ClientSearchModel searchModel)
        {

            if (searchModel == null)
                searchModel = new ClientSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ClientViewModel>> Msg = new ResponseMsg<IEnumerable<ClientViewModel>>();

            Msg.Content = db.Clients
                .Include(i => i.PublicUser)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new ClientViewModel(m));
            Msg.TotalCount = db.Clients.Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ClientViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {

            ResponseMsg<ClientViewModel> Msg = new ResponseMsg<ClientViewModel>();
            var item = db.Clients.Include(i => i.PublicUser).FirstOrDefault(i=> i.Id ==id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new ClientViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

       
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
