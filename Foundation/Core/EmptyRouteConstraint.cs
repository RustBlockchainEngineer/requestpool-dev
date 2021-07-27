using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Foundation.Core
{
        public class LocalhostConstraint : IRouteConstraint
        {
            public bool Match(
                    HttpContextBase httpContext,
                    Route route,
                    string parameterName,
                    RouteValueDictionary values,
                    RouteDirection routeDirection)
            {
                return httpContext.Request.IsLocal;
            /*
             routes.MapRoute(
                "Admin",
                "Admin/{action}",
                new {controller="Admin"},
                new {isLocal=new LocalhostConstraint()}
            );
             */

            }
        }

    public class NotWebApiConstraint : IRouteConstraint
    {
        public bool Match(
                HttpContextBase httpContext,
                Route route,
                string parameterName,
                RouteValueDictionary values,
                RouteDirection routeDirection)
        {
            return values["controller"].ToString() != "api" && values["controller"].ToString() != "admin"
                && values["action"].ToString() != "api" && values["action"].ToString() != "admin";
        }
    }
}