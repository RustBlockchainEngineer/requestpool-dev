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
using System.IO;
using System.Web.Http.ModelBinding;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/publicusers")]
    [CustomAuthorize(SystemRoles.ManagePublicUsers)]

    public class PublicUsersController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private PublicUserManager _userManager;
        private AdministratorUserManager _administratorUserManager;

        private ApplicationRoleManager _roleManager;

        public PublicUserManager PublicUserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<PublicUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? new ApplicationRoleManager(new ApplicationRoleStore(db));
            }
            private set
            {
                _roleManager = value;
            }
        }

        public AdministratorUserManager AdministratorUserManager
        {
            get
            {
                return _administratorUserManager ?? Request.GetOwinContext().GetUserManager<AdministratorUserManager>();
            }
            private set
            {
                _administratorUserManager = value;
            }
        }

        public PublicUsersController()
        {
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<PublicUserViewModel>>))]
        [HttpGet]
        [Route("")]
        public HttpResponseMessage All()
        {
            ResponseMsg<IEnumerable<PublicUserViewModel>> Msg = new ResponseMsg<IEnumerable<PublicUserViewModel>>();
            Msg.Content = db.PublicUsers
               .Include(m => m.Creator)
               .ToList().
               Select(m => new PublicUserViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<PublicUserViewModel>>))]
        [HttpGet]
        [Route("search")]
        public HttpResponseMessage Get([FromUri]PublicUserSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new PublicUserSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<PublicUserViewModel>> Msg = new ResponseMsg<IEnumerable<PublicUserViewModel>>();
            Msg.Content = db.PublicUsers
               .Include(m => m.Creator)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new PublicUserViewModel(m));
            Msg.TotalCount = db.PublicUsers.Where(searchModel.Search).Count();
            
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<PublicUserViewModel>))]
        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Get(long id)
        {
            ResponseMsg<PublicUserViewModel> Msg = new ResponseMsg<PublicUserViewModel>();
            var user = db.PublicUsers.Find(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new PublicUserViewModel(user);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<PublicUserViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(PublicUserPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var currentUserId = User.Identity.GetUserId<long>();

            var user = await PublicUserManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.public_user_user_exists;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var item = new Otp();
            item.CreationDate = DateTime.UtcNow;
            item.IP = Helper.GetClientIP();
            item.Code = Activation.CreateCode();
            if(!String.IsNullOrEmpty(model.PhoneNumber))
                item.Phone = model.PhoneNumber.Trim();
            item.Username = model.Username.Trim();
            item.IsSetByAdmin = true;
            item.Purpose = OtpPurposes.Login;
            item.CreatorId = AdministratorUserManager.FindByName(SystemRoles.System).Id;
            db.Otp.Add(item);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            user = new PublicUser();
            model.UpdateModel(user);
            user.CreationDate = user.LastUpdateDate = DateTime.UtcNow;
            user.CreatorId = currentUserId;
            user.Otp = item.Code;

            IdentityResult result = await PublicUserManager.CreateAsync(user, user.Otp);
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
            await PublicUserManager.AddToRoleAsync(user.Id, SystemRoles.PublicUser);
            MembershipPlan defaultMembershipPlan = db.MembershipPlans.FirstOrDefault(m => m.IsActive && !m.IsDeleted && m.IsDefault);
            if (defaultMembershipPlan != null)
            {
                Membership defaultMembership = new Membership();
                defaultMembership.MembershipPlanId = defaultMembershipPlan.Id;
                defaultMembership.PublicUserId = user.Id;
                defaultMembership.StartDate = DateTime.UtcNow;
                defaultMembership.EndDate = DateTime.UtcNow.AddDays(AppSettings.DbSettings.DefaultMembershipPeriod);
                defaultMembership.CreatorId = currentUserId;
                db.Memberships.Add(defaultMembership);
            }
            try
            {
                db.SaveChanges();
            }
            catch
            {
                // fail silently
            }
            if (model.PhotoFile != null)
            {

                String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetPublicUserFolder());
                String FileName = user.Id + "-" + DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ffffff") + Path.GetExtension(model.PhotoFile.OriginalFileName);


                if (UploadHelper.IsValidFile(FileName))
                {

                    try
                    {
                        if (!Directory.Exists(DirectoryPath))
                            Directory.CreateDirectory(DirectoryPath);
                        File.WriteAllBytes(DirectoryPath + "/" + FileName, Convert.FromBase64String(model.PhotoFile.Content));
                        user.Photo = FileName;
                    }
                    catch (Exception ex)
                    {
                        ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                        ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                        return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                    }
                    await PublicUserManager.UpdateAsync(user);
                }
            }
            ResponseMsg<PublicUserViewModel> Msg = new ResponseMsg<PublicUserViewModel>();
            db.PublicUsers.Attach(user);
            db.Entry(user).Reference(u => u.Creator).Load();

            Msg.Content = new PublicUserViewModel(user);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<PublicUserViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, PublicUserPutModel model)
        {

            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var user = await PublicUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(user);
            user.LastUpdateDate = DateTime.UtcNow;
            await PublicUserManager.AddToRoleAsync(user.Id, SystemRoles.PublicUser);
            if (model.PhotoFile != null)
            {

                String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetPublicUserFolder());
                String FileName = user.Id + "-" + DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ffffff") + Path.GetExtension(model.PhotoFile.OriginalFileName);
                try
                {
                    if (!Directory.Exists(DirectoryPath))
                        Directory.CreateDirectory(DirectoryPath);
                    File.WriteAllBytes(DirectoryPath + "/" + FileName, Convert.FromBase64String(model.PhotoFile.Content));
                    user.Photo = FileName;
                }
                catch (Exception ex)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                    ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
                await PublicUserManager.UpdateAsync(user);
            }
            IdentityResult result = await PublicUserManager.UpdateAsync(user);
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

            ResponseMsg<PublicUserViewModel> Msg = new ResponseMsg<PublicUserViewModel>();
            Msg.Content = new PublicUserViewModel(user);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<PublicUserViewModel>))]
        [HttpPut]
        [Route("{id}/update-code")]
        public async Task<HttpResponseMessage> PutActivationCode(long id)
        {
            var user = await PublicUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var item = new Otp();
            item.CreationDate = DateTime.UtcNow;
            item.IP = Helper.GetClientIP();
            item.Code = Activation.CreateCode();
            item.Username = user.UserName.Trim();
            if (!String.IsNullOrEmpty(user.PhoneNumber))
                item.Phone = user.PhoneNumber.Trim();
            item.IsSetByAdmin = true;
            item.Purpose = OtpPurposes.Login;
            item.CreatorId = AdministratorUserManager.FindByName(SystemRoles.System).Id;
            db.Otp.Add(item);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            try
            {
                user.Otp = item.Code;
                await PublicUserManager.UpdateAsync(user);
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            Msg.Content = user.Otp;
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }
        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var user = await PublicUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            user.UserName = "[X]" + user.UserName + "|" + DateTime.UtcNow.ToString("dd-MM-YYYY-hh-mm-ss");
            user.IsDeleted = true;
            IdentityResult result = await PublicUserManager.UpdateAsync(user);
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
        [HttpDelete]
        [Route("{id}/photo")]
        public async Task<HttpResponseMessage> DeletePhoto(long id)
        {

            var user = await PublicUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            string fileName = user.Photo;
            user.Photo = null;
            IdentityResult result = await PublicUserManager.UpdateAsync(user);
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
            String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetPublicUserFolder());
            try
            {
                File.Move(DirectoryPath + "/" + fileName, DirectoryPath + "/X-" + fileName);
            }
            catch (Exception ex)
            {
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpPost]
        [Route("{id}/toggleactive")]
        public async Task<HttpResponseMessage> Activate(long id)
        {
            var user = await PublicUserManager.FindByIdAsync(id);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            user.IsActive = !user.IsActive;
            IdentityResult result = await PublicUserManager.UpdateAsync(user);
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
