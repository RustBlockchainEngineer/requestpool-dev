using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Foundation.Core
{
    public class MvcLocalizedControllerActivator : IControllerActivator
    {

        public IController Create(RequestContext requestContext, Type controllerType)
        {
            var routeCulture = requestContext.RouteData.Values["culture"] ?? "en";
            CultureInfo cultureInfo = null;

            try
            {
                cultureInfo = new CultureInfo(routeCulture.ToString());
            }
            catch
            {
                cultureInfo = new CultureInfo("en");
            }

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);

            return DependencyResolver.Current.GetService(controllerType) as IController;
        }
    }
}