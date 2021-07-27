using Foundation.Core;
using System.Web.Mvc;

namespace Foundation.Areas.Public
{
    public class PublicAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Public";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            ////context.MapRoute(
            ////    name: "Public_with_culture",
            ////    url: "{culture}/public/{controller}/{action}/{id}",
            ////    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            ////    constraints: new { webapi = new NotWebApiConstraint(), culture = @"(\w{2})|(\w{2}-\w{2})", id = @"(\d+)?" },   // en or en-US
            ////    namespaces: new string[] { "Foundation.Areas.Public.Controllers" }
            ////);
            context.MapRoute(
                name: "Public_default",
                url: "public/{controller}/{action}/{id}",
                defaults: new { culture = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { webapi = new NotWebApiConstraint(), id = @"(\d+)?" },
                namespaces: new string[] { "Foundation.Areas.Public.Controllers" }
            );
        }
    }
}