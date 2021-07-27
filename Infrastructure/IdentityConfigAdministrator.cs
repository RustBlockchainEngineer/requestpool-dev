using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Domain;
using Domain.Models;

namespace Infrastructure
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class AdministratorUserManager : UserManager<Administrator, long>
    {
        public AdministratorUserManager(AdministratorUserStore store) : base(store)
        {
            Config(this);
        }
        private static void Config(AdministratorUserManager manager)
        {
            manager.UserValidator = new UserValidator<Administrator, long>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 5,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            //manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser,long>
            //{
            //    MessageFormat = "Your security code is {0}"
            //});
            //manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser,long>
            //{
            //    Subject = "Security Code",
            //    BodyFormat = "Your security code is {0}"
            //});
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();

        }
        public static AdministratorUserManager Create(IdentityFactoryOptions<AdministratorUserManager> options, IOwinContext context)
        {
            var manager = new AdministratorUserManager(new AdministratorUserStore(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            Config(manager);
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<Administrator, long>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }
    // Configure the application sign-in manager which is used in this application.
    public class AdministratorSignInManager : SignInManager<Administrator, long>
    {
        public AdministratorSignInManager(AdministratorUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(Administrator user)
        {
            return user.GenerateUserIdentityAsync((AdministratorUserManager)UserManager);
        }

        public static AdministratorSignInManager Create(IdentityFactoryOptions<AdministratorSignInManager> options, IOwinContext context)
        {
            return new AdministratorSignInManager(context.GetUserManager<AdministratorUserManager>(), context.Authentication);
        }
    }
    
    public class AdministratorUserStore : UserStore<Administrator, ApplicationRole, long, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public AdministratorUserStore(ApplicationDbContext context) : base(context){}
        public static AdministratorUserStore Create(IdentityFactoryOptions<AdministratorUserStore> options, IOwinContext context)
        {
            return new AdministratorUserStore(context.Get<ApplicationDbContext>());
        }
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole, long, ApplicationUserRole>
    {
        public ApplicationRoleStore(ApplicationDbContext context) : base(context)
        {
        }
    }
    public class ApplicationRoleManager : RoleManager<ApplicationRole, long>, IDisposable
    {
        public ApplicationRoleManager(ApplicationRoleStore store) : base(store) { }
        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }
}
