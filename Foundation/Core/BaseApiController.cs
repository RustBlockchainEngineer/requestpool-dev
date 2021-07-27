using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using System.Web.Http.Tracing;
using System.Web.Http.Cors;
using System.Web.Http.Controllers;
using System.Globalization;
using System.Threading;

namespace MobileServies.Core
{
    [EnableCors(origins: "*", headers: "*", methods: "GET, POST, PUT, DELETE, OPTIONS", SupportsCredentials = true)]
    public class BaseApiController: ApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            //var routeCulture = controllerContext.Request.GetRouteData().Values["culture"] ?? "en";
            //CultureInfo cultureInfo = null;

            //try
            //{
            //    cultureInfo = new CultureInfo(routeCulture.ToString());
            //}
            //catch
            //{
            //    cultureInfo = new CultureInfo("en");
            //}

            //Thread.CurrentThread.CurrentUICulture = cultureInfo;
            //Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            base.Initialize(controllerContext);
        }
    }
}