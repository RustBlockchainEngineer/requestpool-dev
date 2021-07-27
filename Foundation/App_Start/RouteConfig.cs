using Foundation.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Foundation
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapMvcAttributeRoutes();
            AreaRegistration.RegisterAllAreas();

            //disable multilang support
            //routes.MapRoute(
            //    name: "Default_with_culture",
            //    url: "{culture}/{controller}/{action}/{id}",
            //    defaults: new { area = "Public", controller = "Home", action = "Index", id = UrlParameter.Optional },
            //    constraints: new { webapi = new NotWebApiConstraint(), culture = @"(\w{2})|(\w{2}-\w{2})", id = @"(\d+)?" },   // en or en-US
            //    namespaces: new string[] { "Foundation.Areas.Front.Controllers" }
            //).DataTokens.Add("area", "Front");
            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { area = "Front", culture = "en", controller = "Home", action = "Index", id = UrlParameter.Optional },
               namespaces: new string[] { "Foundation.Areas.Front.Controllers" },
               constraints: new { webapi = new NotWebApiConstraint(), id = @"(\d+)?" }
           ).DataTokens.Add("area", "Front");


        }
    }
}
