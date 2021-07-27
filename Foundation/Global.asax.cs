using Foundation.Core;
using SmartAdminMvc.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Foundation
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //var binder = new DateTimeModelBinder("dd-MM-yyyy");
            //ModelBinders.Binders.Add(typeof(DateTime), binder);
            //ModelBinders.Binders.Add(typeof(DateTime?), binder);
            AppSettings.Load();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            

            ControllerBuilder.Current.SetControllerFactory(new DefaultControllerFactory(new MvcLocalizedControllerActivator()));
            

        }

      

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            /*
            var handler = Context.Handler as MvcHandler;
            var routeData = handler != null ? handler.RequestContext.RouteData : null;
            var routeCulture = (routeData != null && routeData.Values["culture"] != null) ? routeData.Values["culture"].ToString() : null;
            //var languageCookie = HttpContext.Current.Request.Cookies["culture"];

            // so far, if routeCulture is null, then it is a web api request; so we check the http header
            //if (routeCulture == null && HttpContext.Current.Request.UserLanguages != null)
            //{
            //    routeCulture = HttpContext.Current.Request.UserLanguages[0];
            //}
            CultureInfo cultureInfo = null;
            try
            {
                cultureInfo = new CultureInfo(routeCulture ?? "en");
            }
            catch
            {
                cultureInfo = new CultureInfo("en");
            }

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            */
        }
    }
}
