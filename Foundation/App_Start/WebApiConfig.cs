using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using Foundation.Core;
using System.Web.Http.Controllers;
using SmartAdminMvc.Core;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Http.ModelBinding;

namespace Foundation
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Services.Insert(typeof(ModelBinderProvider), 0,
            //    new SimpleModelBinderProvider(typeof(DateTime), new WebApiDateTimeModelBinder("dd-MM-yyyy")));
            //config.Services.Insert(typeof(ModelBinderProvider), 0,
            //    new SimpleModelBinderProvider(typeof(DateTime?), new WebApiDateTimeModelBinder("dd-MM-yyyy")));

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            config.MessageHandlers.Add(new ApiLocalizationMessageHandler());

            //config.EnableCors();
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            
            
            // Web API routes
            config.MapHttpAttributeRoutes();
            //config.Services.Replace(typeof(IHttpActionSelector), new ApiLocalizedActionSelector());
            /*
            config.Routes.MapHttpRoute(
                 name: "AdminApi__with_culture",
                 routeTemplate: "{culture}/admin/api/{controller}/{id}",
                 defaults: new
                 {
                     id = RouteParameter.Optional,
                     Namespaces = new string[] { "Foundation.Areas.Admin.Controllers.API" }
                 },
                 constraints: new { culture = @"(\w{2})|(\w{2}-\w{2})" }   // en or en-US
             );
             /*
            config.Routes.MapHttpRoute(
                name: "AdminApi",
                routeTemplate: "admin/api/{controller}/{id}",
                defaults: new
                {
                    culture = "en",
                    id = RouteParameter.Optional,
                    Namespaces = new string[] { "Foundation.Areas.Admin.Controllers.API" }
                }
            );
            //front api routes
            config.Routes.MapHttpRoute(
                name: "FrontApi_with_culture",
                routeTemplate: "{culture}/front/api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    Namespaces = new string[] { "Foundation.Areas.Front.Controllers.API" }
                },
                constraints: new { culture = @"(\w{2})|(\w{2}-\w{2})" }   // en or en-US
            );
            config.Routes.MapHttpRoute(
                name: "FrontApi",
                routeTemplate: "front/api/{controller}/{id}",
                defaults: new
                {
                    culture = "en",
                    id = RouteParameter.Optional,
                    Namespaces = new string[] { "Foundation.Areas.Front.Controllers.API" }
                }
            );
            //default api routes
            config.Routes.MapHttpRoute(
                name: "DefaultApi_with_culture",
                routeTemplate: "{culture}/api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional,
                    Namespaces = new string[] { "Foundation.Areas.Admin.Controllers.API" }
                },
                constraints: new { culture = @"(\w{2})|(\w{2}-\w{2})" }   // en or en-US
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    culture = "en",
                    id = RouteParameter.Optional,
                    Namespaces = new string[] { "Foundation.Areas.Admin.Controllers.API" }
                }
            );
            */
        }
    }
}
