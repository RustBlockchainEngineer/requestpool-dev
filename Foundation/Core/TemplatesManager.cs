using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using RazorEngine;
using RazorEngine.Templating; // For extension methods.

namespace Foundation.Core
{
    public static class TemplateKeys
    {
        public static string Otp = "otp";
        public static string Invitation = "invitation";
        public static string Registration = "registration";
        public static string Response = "response";
    }
    public static class TemplatesManager
    {
        public static string Render<M>(string templateKey,M model)
        {
            if (!Engine.Razor.IsTemplateCached(templateKey, typeof(M)))
            {
                if (!File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/"+templateKey+".cshtml")))
                {
                    return null;
                }
                Engine.Razor.Compile(File.ReadAllText(
                    System.Web.Hosting.HostingEnvironment.MapPath("~/Templates/" + templateKey + ".cshtml"))
                    , templateKey, typeof(M));
            }
            return Engine.Razor.Run(templateKey, typeof(M), model);
        }

    }
}