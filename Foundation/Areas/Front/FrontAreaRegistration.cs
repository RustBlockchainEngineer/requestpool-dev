using Foundation.Core;
using System.Web.Mvc;

namespace Foundation.Areas.Front
{
    public class FrontAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Front";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            ////context.MapRoute(
            ////    name: "Front_with_culture",
            ////    url: "{culture}/front/{controller}/{action}/{id}",
            ////    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
            ////    constraints: new { webapi = new NotWebApiConstraint(), culture = @"(\w{2})|(\w{2}-\w{2})", id = @"(\d+)?" },   // en or en-US
            ////    namespaces: new string[] { "Foundation.Areas.Front.Controllers" }
            ////);
            context.MapRoute(
                name: "Front_default",
                url: "front/{controller}/{action}/{id}",
                defaults: new { culture = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { webapi = new NotWebApiConstraint(), id = @"(\d+)?" },
                namespaces: new string[] { "Foundation.Areas.Front.Controllers" }
            );
        }
    }
}