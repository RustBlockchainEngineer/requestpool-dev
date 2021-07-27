using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Foundation.Models;
using Infrastructure;
using Newtonsoft.Json;
using Domain.Models;
using Foundation.Core;

namespace Foundation.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var publicUserManager = context.OwinContext.GetUserManager<PublicUserManager>();
            var adminUserManager = context.OwinContext.GetUserManager<AdministratorUserManager>();
            ApplicationUser user = null;
            if (context.Request.Headers.ContainsKey(AppSettings.PublicLoginHeader))
            {
                ApplicationDbContext db = new ApplicationDbContext();
                user = await publicUserManager.FindAsync(context.UserName, context.Password);
                if(user == null)
                {
                    Otp codeRequest = db.Otp.OrderByDescending(c => c.Id).FirstOrDefault(c =>
                      c.Purpose == OtpPurposes.Login
                      && c.Username == context.UserName
                      && c.Code == context.Password && !c.IsUsed && !c.IsDeleted);
                    if (codeRequest == null)
                    {
                        context.SetError("invalid_grant", "Activation code is incorrect.");
                        return;
                    }
                    user = await publicUserManager.FindByNameAsync(context.UserName);
                    codeRequest.IsUsed = true;
                    codeRequest.UseDate = DateTime.UtcNow;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch { }
                }
            }
            else if (context.Request.Headers.ContainsKey(AppSettings.AdminLoginHeader))//admin
            {
                user = await adminUserManager.FindAsync(context.UserName, context.Password);
            }
            else
            {
                context.SetError("invalid_grant", "Bad Request.");
                return;
            }
            if (user == null || user.IsDeleted)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            if (!user.IsActive || (user.LockoutEnabled && user.LockoutEndDateUtc.HasValue && user.LockoutEndDateUtc >= DateTime.UtcNow ))
            {
                context.SetError("invalid_grant", "This user is locked.");
                return;
            }
            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            List<Claim> roles = oAuthIdentity.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
            string _photo="", _photoUrl="";
            try
            {
                var publicUser  = (PublicUser)user;
                _photo = publicUser.Photo;
                if (!String.IsNullOrEmpty(_photo))
                    _photoUrl = UploadHelper.GetPublicUserUrl(_photo);
            }
            catch { }

            var info = new { name = user.Name, email = user.Email, photo = _photo, photoUrl = _photoUrl};

            AuthenticationProperties properties = CreateProperties(user.UserName,
                JsonConvert.SerializeObject(info),
                JsonConvert.SerializeObject(roles.Select(x => x.Value)));

            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName,string info="{}", string roles = "[]")
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName },
                { "roles",roles},
                { "info",info}
            };
            return new AuthenticationProperties(data);
        }
    }
}