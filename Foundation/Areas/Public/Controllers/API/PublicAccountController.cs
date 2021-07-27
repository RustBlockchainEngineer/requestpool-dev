using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Foundation.Models;
using Foundation.Providers;
using Foundation.Results;
using Infrastructure;
using Foundation.Core;
using System.Web.Http.Description;
using System.Net;
using Domain.Models;
using Foundation.Areas.Public.Models;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Linq;
using System.IO;
using Foundation.Templates;

namespace Foundation.Areas.Public.Controllers.Controllers.API
{
    [RoutePrefix("api/account")]
    public class PublicAccountController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private const string LocalLoginProvider = "Local";
        private PublicSignInManager _signInManager;
        private PublicUserManager _userManager;
        private AdministratorUserManager _administratorUserManager;
        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }


        public PublicAccountController()
        {
        }

        public PublicAccountController(PublicUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            PublicUserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }
        public PublicSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<PublicSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
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

        [AllowAnonymous]
        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpGet]
        [Route("otp/{purpose}")]
        public async Task<HttpResponseMessage> GetCode(string purpose, string username)
        {
            if (username == null || !Regex.IsMatch(username.Trim(), Validation.Email, RegexOptions.ECMAScript))
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_bad_request;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.BadRequest, ErrorMsg);
            }
            var user = await PublicUserManager.FindByNameAsync(username);
            if (user != null && !user.IsActive)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_blocked;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Otp item;
            if (user == null || user.IsDeleted)
            {
                item = CreateOtp( OtpPurposes.Register, username,null);
            }
            else
            {
                item = CreateOtp(OtpPurposes.Login, username,null);
            }
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_generic_internal_server;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            if (!SendOtp(item, user,purpose))
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_generic_internal_server;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            if (purpose == OtpPurposes.Login && (user == null || user.IsDeleted))
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_not_exist;
                ErrorMsg.Code = "does_not_exist";
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            else if (purpose == OtpPurposes.Register && user != null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_exits;
                ErrorMsg.Code = "exists";
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.OK, new ResponseMsg<string>());
        }


        [AllowAnonymous]
        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpPost]
        [Route("register")]
        public async Task<HttpResponseMessage> Register(AccountPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var system = AdministratorUserManager.FindByName(SystemRoles.System);
            var user = await PublicUserManager.FindByNameAsync(model.PhoneNumber);
            if (user != null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_exits;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Otp codeRequest = db.Otp.OrderByDescending(c => c.Id).FirstOrDefault(c =>
            c.Purpose == OtpPurposes.Register
            && c.Username == model.Username.Trim()
            && c.Code == model.ActivationCode && c.IsSent && !c.IsUsed && !c.IsDeleted);
            if (codeRequest == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_invalid_code;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            user = new PublicUser();
            model.UpdateModel(user);
            user.CreationDate = user.LastUpdateDate = DateTime.UtcNow;
            user.CreatorId = system.Id;
            user.Otp = codeRequest.Code;
            user.IsActive = true;
            IdentityResult result = await PublicUserManager.CreateAsync(user, model.Password);
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
                ErrorMsg.Message = Resources.Front.error_generic_internal_server;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            await PublicUserManager.AddToRoleAsync(user.Id, SystemRoles.PublicUser);
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
                    }
                    await PublicUserManager.UpdateAsync(user);
                }
            }

            codeRequest.IsUsed = true;
            codeRequest.UseDate = DateTime.UtcNow;
            try
            {
                db.SaveChanges();
            }
            catch { }

            MembershipPlan defaultMembershipPlan = db.MembershipPlans.FirstOrDefault(m => m.IsActive && !m.IsDeleted && m.IsDefault);
            if(defaultMembershipPlan != null)
            {
                Membership defaultMembership = new Membership();
                defaultMembership.MembershipPlanId = defaultMembershipPlan.Id;
                defaultMembership.PublicUserId = user.Id;
                defaultMembership.StartDate = DateTime.UtcNow;
                defaultMembership.EndDate = DateTime.UtcNow.AddDays(AppSettings.DbSettings.DefaultMembershipPeriod);
                defaultMembership.CreatorId = system.Id;
                db.Memberships.Add(defaultMembership);
            }
            try
            {
                db.SaveChanges();
            }
            catch {
                // fail silently
            }

            MailService.Send(user.Email, 
                Resources.Mail.registration_subject,
                TemplatesManager.Render(TemplateKeys.Registration, new RegistrationTemplateModel()
            {
                Name = user.Name,
                Email = user.Email,
                Url = AppSettings.Server
            }));
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            Msg.Message = Resources.Front.success_registration;
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpGet]
        [Route("profile")]
        public async Task<HttpResponseMessage> GetProfile()
        {
            var currentUserId = User.Identity.GetUserId<long>();

            var user = await PublicUserManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_not_exist;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<AccountViewModel> Msg = new ResponseMsg<AccountViewModel>();
            Msg.Content = new AccountViewModel(user);
            Msg.Message = Resources.Front.success_profile_updated;
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<AccountViewModel>))]
        [HttpPut]
        [Route("profile")]
        public async Task<HttpResponseMessage> UpdateProfile(ProfileFormModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var currentUserId = User.Identity.GetUserId<long>();

            var user = await PublicUserManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_not_exist;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            model.UpdateModel(user);
            if (model.PhotoFile != null)
            {
                String DirectoryPath = System.Web.Hosting.HostingEnvironment.MapPath("~/" + UploadHelper.GetPublicUserFolder());
                String FileName = user.Id + "-" + DateTime.UtcNow.ToString("dd-MM-yyyy-hh-mm-ffffff") + Path.GetExtension(model.PhotoFile.OriginalFileName);
                if (!UploadHelper.IsValidImage(FileName))
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                    ErrorMsg.Message = Resources.Errors.http_bad_request; ;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
                try
                {
                    if (!Directory.Exists(DirectoryPath))
                        Directory.CreateDirectory(DirectoryPath);
                    File.WriteAllBytes(DirectoryPath + "/" + FileName, Convert.FromBase64String(model.PhotoFile.Content));
                    user.Photo = FileName;
                }
                catch (Exception ex)
                {
                }
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
                ErrorMsg.Message = Resources.Front.error_generic_internal_server;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<AccountViewModel> Msg = new ResponseMsg<AccountViewModel>();
            Msg.Content = new AccountViewModel(user);
            Msg.Message = Resources.Front.success_profile_updated;
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpPut]
        [Route("location")]
        public async Task<HttpResponseMessage> UpdateLocation(GelocationPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var currentUserId = User.Identity.GetUserId<long>();

            var user = await PublicUserManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_not_exist;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            user.LatestIP = Helper.GetClientIP();
            if(!String.IsNullOrEmpty(model.Latitude) && !String.IsNullOrEmpty(model.Longitude))
            {
                user.Latitude = model.Latitude;
                user.Longitude = model.Longitude;
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
                ErrorMsg.Message = Resources.Front.error_generic_internal_server;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("photo")]
        public async Task<HttpResponseMessage> DeletePhoto()
        {

            var currentUserId = User.Identity.GetUserId<long>();

            var user = await PublicUserManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_not_exist;
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
                File.Move(DirectoryPath + "/" + fileName, DirectoryPath + "/[deleted]-" + fileName);
            }
            catch (Exception ex)
            {
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<AccountViewModel>))]
        [HttpPut]
        [Route("password")]
        public async Task<HttpResponseMessage> UpdatePassword(PasswordFormModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Front.error_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var currentUserId = User.Identity.GetUserId<long>();

            var user = await PublicUserManager.FindByIdAsync(currentUserId);
            if (user == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Front.error_user_not_exist;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            IdentityResult result = PublicUserManager.ChangePassword(user.Id, model.OldPassword, model.Password);
            if (!result.Succeeded)
            {
                //try using otp
                Otp codeRequest = db.Otp.OrderByDescending(c => c.Id).FirstOrDefault(c =>
                      c.Purpose == OtpPurposes.Login
                      && c.Username == user.UserName
                      && c.Code == model.OldPassword && !c.IsDeleted);
                if (codeRequest == null)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                    ErrorMsg.Message = Resources.Front.error_bad_request;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
                try
                {
                    ApplicationDbContext passwordDb = new ApplicationDbContext();
                    PublicUserStore UserStore = new PublicUserStore(passwordDb);
                    var appUser = passwordDb.PublicUsers.Find(user.Id);
                    String hashedNewPassword = PublicUserManager.PasswordHasher.HashPassword(model.Password);
                    await UserStore.SetPasswordHashAsync(appUser, hashedNewPassword);
                    await UserStore.UpdateAsync(appUser);
                }
                catch (Exception exp)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                    ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
            }
            ResponseMsg<AccountViewModel> Msg = new ResponseMsg<AccountViewModel>();
            Msg.Content = new AccountViewModel(user);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected Otp CreateOtp( string purpose, string username,string phone)
        {
            var item = new Otp();
            item.CreationDate = DateTime.UtcNow;
            item.IP = Helper.GetClientIP();
            item.Code = Activation.CreateCode();
            if(!String.IsNullOrEmpty(username))
                item.Username = username.Trim();
            if(!String.IsNullOrEmpty(phone))
                item.Phone = phone.Trim();
            item.Purpose = purpose;
            item.CreatorId = AdministratorUserManager.FindByName(SystemRoles.System).Id;
            db.Otp.Add(item);
            try
            {
                db.SaveChanges();
                return item;
            }
            catch (Exception exp)
            {
            }
            return null;
        }

        protected bool SendOtp(Otp item, PublicUser user, string purpose)
        {
            bool result = false;
            string subject = purpose == OtpPurposes.Login? Resources.Mail.otp_subject: Resources.Mail.activation_subject;
            item.IsSent = MailService.Send(item.Username,
                subject,
                TemplatesManager.Render(TemplateKeys.Otp, new OtpTemplateModel()
            {
                Otp= item.Code
            }));
            try
            {
                db.SaveChanges();
                if (user != null)
                {
                    user.Otp = item.Code;
                    PublicUserManager.Update(user);
                }
                result = true;
            }
            catch (Exception exp)
            {
            }
            return result;
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
