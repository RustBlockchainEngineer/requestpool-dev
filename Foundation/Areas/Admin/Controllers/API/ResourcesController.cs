using Foundation.Areas.Admin.Models;
using Foundation.Areas.Admin.Models.ViewModels;
using Foundation.Core;
using MobileServies.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Foundation.Areas.Admin.Controllers.API
{
    //[RoutePrefix("{culture}/admin/api/translations/")]
    [RoutePrefix("admin/api/resources")]
    public class ResourcesController : ApiAdminController
    {
        [HttpGet]
        [AllowAnonymous]
        //[Route("{culture}/admin/api/translations/")]
        [Route("")]
        public IHttpActionResult Get()
        {
            Dictionary<string, string> SharedDict = new Dictionary<string, string>();
            var SharedResourceSet = Resources.Shared.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            var AdminSharedResourceSet = Resources.Admin.Shared.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            PopulateDictionary(SharedDict, SharedResourceSet);
            PopulateDictionary(SharedDict, AdminSharedResourceSet);
            //
            Dictionary<string, string> MenuDict = new Dictionary<string, string>();
            var MenuResourceSet = Resources.Menu.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            var AdminMenuResourceSet = Resources.Admin.Menu.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            PopulateDictionary(MenuDict, MenuResourceSet);
            PopulateDictionary(MenuDict, AdminMenuResourceSet);

            //
            var ErrorsResourceSet = Resources.Errors.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            var AdminErrorsResourceSet = Resources.Admin.Errors.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            Dictionary<string, string> ErrorsDict = new Dictionary<string, string>();
            PopulateDictionary(ErrorsDict, ErrorsResourceSet);
            PopulateDictionary(ErrorsDict, AdminErrorsResourceSet);
           
            //
           
            //
            var ModelsResourceSet = Resources.Models.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            var AdminModelsResourceSet = Resources.Admin.Models.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            Dictionary<string, string> ModelsDict = new Dictionary<string, string>();
            PopulateDictionary(ModelsDict, ModelsResourceSet);
            PopulateDictionary(ModelsDict, AdminModelsResourceSet);

           
            //
            var ViewsResourceSet = Resources.Views.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            var AdminViewsResourceSet = Resources.Admin.Views.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            Dictionary<string, string> ViewsDict = new Dictionary<string, string>();
            PopulateDictionary(ViewsDict, ViewsResourceSet);
            PopulateDictionary(ViewsDict, AdminViewsResourceSet);

           
            //
            var RolesResourceSet = Resources.Admin.Roles.ResourceManager.GetResourceSet(Thread.CurrentThread.CurrentUICulture, true, true);
            Dictionary<string, string> RolesDict = new Dictionary<string, string>();
            PopulateDictionary(RolesDict, RolesResourceSet);


            return base.Ok(new
            {
                shared = SharedDict,
                menu = MenuDict,
                errors = ErrorsDict,
                roles = RolesDict,
                views = ViewsDict,
                models = ModelsDict,
                validation = new
                {
                    common = this.GetConstants(typeof(Foundation.Core.Validation)),
                    model = this.GetConstants(typeof(Foundation.Areas.Admin.ModelValidation))
                }
            });
        }

        protected void PopulateDictionary(Dictionary<string,string> dictionary, System.Resources.ResourceSet resourceSet)
        {
            IDictionaryEnumerator enumerator = resourceSet.GetEnumerator();
            while (enumerator.MoveNext())
            {
                string key = enumerator.Key.ToString();
                if (dictionary.ContainsKey(key))
                {
                    if (key.StartsWith("x_"))
                    {
                        dictionary.Remove(key);
                    }
                    else
                    {
                        dictionary[key] = enumerator.Value.ToString();
                    }
                }
                else
                {
                    dictionary.Add(key, enumerator.Value.ToString());
                }
            }
            
        }

        protected Dictionary<string, string> GetConstants(Type type)
        {
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            var props = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly).ToList();
            foreach (var prop in props)
            {
                Dict.Add(prop.Name, (prop.GetRawConstantValue() as string));//.Replace("\\","\\\\"));
            }

            return Dict;
        }
    }
}
