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
using Domain.Models;
using Foundation.Areas.Admin.Models;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/administrators")]
    [CustomAuthorize(SystemRoles.ManageAdmins)]
    public class AdministratorsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AdministratorUserManager _userManager;
        private AdministratorUserStore _userStore;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }
        public AdministratorUserManager AdministratorUserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<AdministratorUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public AdministratorUserStore UserStore
        {
            get
            {
                return _userStore ?? new AdministratorUserStore(db);
            }
            private set
            {
                _userStore = value;
            }
        }


        public AdministratorsController()
        {
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<AdministratorViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage All()
        {
            ResponseMsg<IEnumerable<AdministratorViewModel>> Msg = new ResponseMsg<IEnumerable<AdministratorViewModel>>();
            Msg.Content = db.Administrators
              .Include(m => m.Creator)
              .Where(i => i.UserName != SystemRoles.Super && i.UserName != SystemRoles.System)
              .ToList()
              .Select(m => new AdministratorViewModel(m, AdministratorUserManager.GetRoles(m.Id).ToArray()));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<AdministratorViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Get([FromUri]AdministratorSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new AdministratorSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<AdministratorViewModel>> Msg = new ResponseMsg<IEnumerable<AdministratorViewModel>>();

            Msg.Content = db.Administrators
              .Include(m => m.Creator)
              .Where(searchModel.Search)
              .OrderByDescending(i => i.Id)
              .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
              .ToList()
              .Select(m => new AdministratorViewModel(m, AdministratorUserManager.GetRoles(m.Id).ToArray()));

            Msg.TotalCount = db.Administrators.Where(searchModel.Search).Count();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<AdministratorViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {
            ResponseMsg<AdministratorViewModel> Msg = new ResponseMsg<AdministratorViewModel>();
            var user = db.Administrators.Find(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new AdministratorViewModel(user, AdministratorUserManager.GetRoles(id).ToArray());
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<AdministratorViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(AdministratorPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var currentUserId = User.Identity.GetUserId<long>();

            var user = await AdministratorUserManager.FindByNameAsync(model.Username);
            if(user != null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Admin.Errors.administrator_user_exists;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            user = new Administrator();
            model.UpdateModel(user);
            user.CreationDate = user.LastUpdateDate = DateTime.UtcNow;
            user.CreatorId = currentUserId;
               
            IdentityResult result = await AdministratorUserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            var applicationRoles = db.Roles.ToList();
            var userRoles = new List<string>();
            if(model.Roles != null)
            {
                foreach (string roleName in model.Roles)
                {
                    if (applicationRoles.Exists(r => r.Name == roleName)
                        && !userRoles.Contains(roleName))
                    {
                        userRoles.Add(roleName);
                    }
                }
            }
           
            if (!userRoles.Contains(SystemRoles.Admin))
            {
                userRoles.Add(SystemRoles.Admin);
            }
            foreach(string roleName in userRoles)
            {
                await AdministratorUserManager.AddToRoleAsync(user.Id, roleName);
            }
            ResponseMsg<AdministratorViewModel> Msg = new ResponseMsg<AdministratorViewModel>();
            db.Administrators.Attach(user);
            db.Entry(user).Reference(u => u.Creator).Load();
            
            //var roles = AdministratorUserManager.GetRoles(user.Id).ToArray();
            Msg.Content = new AdministratorViewModel(user, userRoles.ToArray());
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<AdministratorViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, AdministratorPutModel model)
        {
            
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var user = await AdministratorUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(user);
            user.LastUpdateDate = DateTime.UtcNow;
            IdentityResult result = await AdministratorUserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }

            await AdministratorUserManager.RemoveFromRolesAsync(user.Id, AdministratorUserManager.GetRoles(user.Id).ToArray());
            var applicationRoles = db.Roles.ToList();
            var userRoles = new List<string>();
            foreach (string roleName in model.Roles)
            {
                if (applicationRoles.Exists(r => r.Name == roleName)
                    && !userRoles.Contains(roleName))
                {
                    userRoles.Add(roleName);
                }
            }
            if (!userRoles.Contains(SystemRoles.Admin))
            {
                userRoles.Add(SystemRoles.Admin);
            }
            foreach (string roleName in userRoles)
            {
                await AdministratorUserManager.AddToRoleAsync(user.Id, roleName);
            }
            if (model.IsUpdatePassword)
            {
                try
                {
                    ApplicationDbContext passwordDb = new ApplicationDbContext();
                    AdministratorUserStore UserStore = new AdministratorUserStore(passwordDb);
                    var appUser = passwordDb.Administrators.Find(user.Id);
                    String hashedNewPassword = AdministratorUserManager.PasswordHasher.HashPassword(model.Password);
                    await UserStore.SetPasswordHashAsync(appUser, hashedNewPassword);
                    await UserStore.UpdateAsync(appUser);
                }
                catch(Exception exp)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                    ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
            }
            ResponseMsg<AdministratorViewModel> Msg = new ResponseMsg<AdministratorViewModel>();
            Msg.Content = new AdministratorViewModel(user,
                            AdministratorUserManager.GetRoles(user.Id).ToArray());
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var user = await AdministratorUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            user.UserName = "[X]" + user.UserName+"|"+DateTime.UtcNow.ToString("dd-MM-YYYY-hh-mm-ss");
            user.IsDeleted = true;
            IdentityResult result = await AdministratorUserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        
        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpPost]
        [Route("{id}/toggleactive")]
        public async Task<HttpResponseMessage> Activate(long id)
        {
            var user = await AdministratorUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            user.IsActive = !user.IsActive;
            IdentityResult result = await AdministratorUserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.OK, new ResponseMsg<string>());
        }




        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

    }
}
