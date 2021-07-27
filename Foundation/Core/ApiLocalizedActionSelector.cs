using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Foundation.Core
{
    public class ApiLocalizedActionSelector: ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)

        {
            var routeData = controllerContext.RouteData;
            if (routeData.Values.ContainsKey("MS_SubRoutes"))
            {
                routeData = ((IHttpRouteData[])routeData.Values["MS_SubRoutes"]).First();
            }
            
            CultureInfo cultureInfo = new CultureInfo("en");
            try
            {
                cultureInfo = new CultureInfo(routeData.Values["culture"].ToString());
            }
            catch
            {
            }
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            return base.SelectAction(controllerContext);
        }
    }
}