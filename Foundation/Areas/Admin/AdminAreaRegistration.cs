using Foundation.Core;
using System.Web.Mvc;

namespace Foundation.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                name: "Admin_with_culture",
                url: "{culture}/admin/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { webapi = new NotWebApiConstraint(), culture = @"(\w{2})|(\w{2}-\w{2})", id = @"(\d+)?" },   // en or en-US , controller = "(?!api).*" 
                namespaces: new string[] { "Foundation.Areas.Admin.Controllers" }
            );
            context.MapRoute(
                name: "Admin_default",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { culture = "ar", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { webapi = new NotWebApiConstraint(), id = @"(\d+)?" },// controller = "(?!api).*"
                namespaces: new string[] { "Foundation.Areas.Admin.Controllers" }
            );
        }
    }
}