using Foundation.Core;
using System.Web;
using System.Web.Optimization;

namespace Foundation
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            var OrderedBundler = new CustomBundleOrderer(true);

            //bundles.Add(new ScriptBundle("~/scripts/admin/app/ui/")
            //    .Include("~/App/ui/app.ui.js")
            //    .IncludeDirectory("~/App/ui/directives", "*.js", true)
            //    .IncludeDirectory("~/App/ui/filters", "*.js", true)
            //    .IncludeDirectory("~/App/ui/services", "*.js", true)
            //    .IncludeDirectory("~/App/ui/controllers", "*.js", true));

            //bundles.Add(new ScriptBundle("~/scripts/admin/app/core/")
            //    .Include("~/App/core/app.core.js")
            //    .IncludeDirectory("~/App/core/directives", "*.js", true)
            //    .IncludeDirectory("~/App/core/filters", "*.js", true)
            //    .IncludeDirectory("~/App/core/services", "*.js", true)
            //    .IncludeDirectory("~/App/core/controllers", "*.js", true));
            //bundles.Add(new ScriptBundle("~/scripts/admin/app/main/")
            //    .Include("~/App/main/app.main.js")
            //    .IncludeDirectory("~/App/main/directives", "*.js", true)
            //    .IncludeDirectory("~/App/main/filters", "*.js", true)
            //    .IncludeDirectory("~/App/main/services", "*.js", true)
            //    .IncludeDirectory("~/App/main/controllers", "*.js", true));

            bundles.Add(new ScriptBundle("~/wwwroot/wysiwyg/")
                .Include("~/wwwroot/tinymce/tinymce.js"));
            bundles.Add(new ScriptBundle("~/wwwroot/excel/")
                .Include("~/wwwroot/lib/js-xlsx.core.js"));

            var scriptAdminLib = new ScriptBundle("~/scripts/admin/lib/")
                .Include("~/wwwroot/lib/fastclick.js")
                .Include("~/wwwroot/lib/jquery.js")
                .Include("~/wwwroot/lib/overlay.js")
                .Include("~/wwwroot/lib/jquery.slimscroll.js")
                .Include("~/wwwroot/lib/angular.js")
                .Include("~/wwwroot/lib/angular-ui-router.js")
                .Include("~/wwwroot/lib/angular-sanitize.js")
                .Include("~/wwwroot/lib/angular-slimscroll.js")
                .Include("~/wwwroot/lib/moment-with-locales.js")
                .Include("~/wwwroot/lib/angular-moment-picker.js")
                .Include("~/wwwroot/lib/ui-bootstrap-tpls.js")
                .Include("~/wwwroot/lib/smart-table.js")
                .Include("~/wwwroot/lib/checklist-model.js")
                .Include("~/wwwroot/lib/tinymce.angular.js")
                .Include("~/wwwroot/lib/ie10-viewport-bug-workaround.js")
                .Include("~/wwwroot/lib/ui-select.js")
                .Include("~/wwwroot/lib/tinycolor.js")
                .Include("~/wwwroot/lib/angularjs-color-picker.js")
                .Include("~/wwwroot/lib/admin-lte-support.js")
                .Include("~/wwwroot/lib/ng-map.js")
                .Include("~/wwwroot/lib/ui-mask.js")
                .Include("~/wwwroot/lib/handsontable.full.js")
                .Include("~/wwwroot/lib/ngHandsontable.js")
                .Include("~/wwwroot/lib/file-saver.js")
                .Include("~/wwwroot/lib/ngGeolocation.js")
                .Include("~/wwwroot/lib/lodash.js")
                .Include("~/wwwroot/lib/angularjs-dropdown-multiselect.js")
                ;
            bundles.Add(scriptAdminLib);
            bundles.Add(new ScriptBundle("~/scripts/admin/app/")
                //ui
                .Include("~/App/ui/app.ui.js")
                .IncludeDirectory("~/App/ui/directives", "*.js", true)
                .IncludeDirectory("~/App/ui/filters", "*.js", true)
                .IncludeDirectory("~/App/ui/services", "*.js", true)
                .IncludeDirectory("~/App/ui/controllers", "*.js", true)
                //core
                .Include("~/App/core/app.core.js")
                .IncludeDirectory("~/App/core/directives", "*.js", true)
                .IncludeDirectory("~/App/core/filters", "*.js", true)
                .IncludeDirectory("~/App/core/services", "*.js", true)
                .IncludeDirectory("~/App/core/controllers", "*.js", true)
                 //main
                 .Include("~/App/main/app.main.js")
                .IncludeDirectory("~/App/main/directives", "*.js", true)
                .IncludeDirectory("~/App/main/filters", "*.js", true)
                .IncludeDirectory("~/App/main/services", "*.js", true)
                .IncludeDirectory("~/App/main/controllers", "*.js", true)
                //app
                .Include("~/App/app.js"));

            var styleAdminLibLtr = new StyleBundle("~/wwwroot/admin/css/lib/ltr").Include("~/wwwroot/css/*-ltr.css");
            styleAdminLibLtr.Orderer = OrderedBundler;
            bundles.Add(styleAdminLibLtr);
            var styleAdminLibRtl = new StyleBundle("~/wwwroot/admin/css/lib/rtl").Include("~/wwwroot/css/*-rtl.css");
            styleAdminLibRtl.Orderer = OrderedBundler;
            bundles.Add(styleAdminLibRtl);


            var styleAdminLtr = new StyleBundle("~/wwwroot/admin/css/ltr").Include("~/wwwroot/_admin/css/*-ltr.css");
            styleAdminLtr.Orderer = OrderedBundler;
            bundles.Add(styleAdminLtr);
            var styleAdminRtl = new StyleBundle("~/wwwroot/admin/css/rtl").Include("~/wwwroot/_admin/css/*-rtl.css");
            styleAdminRtl.Orderer = OrderedBundler;
            bundles.Add(styleAdminRtl);
            /*****************/
            var scriptPublicLib = new ScriptBundle("~/scripts/public/lib/")
                .Include("~/wwwroot/lib/fastclick.js")
                .Include("~/wwwroot/lib/jquery.js")
                .Include("~/wwwroot/lib/overlay.js")
                .Include("~/wwwroot/lib/jquery.slimscroll.js")
                .Include("~/wwwroot/lib/angular.js")
                .Include("~/wwwroot/lib/angular-ui-router.js")
                .Include("~/wwwroot/lib/angular-sanitize.js")
                .Include("~/wwwroot/lib/angular-slimscroll.js")
                .Include("~/wwwroot/lib/moment-with-locales.js")
                .Include("~/wwwroot/lib/angular-moment-picker.js")
                .Include("~/wwwroot/lib/ui-bootstrap-tpls.js")
                .Include("~/wwwroot/lib/smart-table.js")
                .Include("~/wwwroot/lib/checklist-model.js")
                .Include("~/wwwroot/lib/tinymce.angular.js")
                .Include("~/wwwroot/lib/ie10-viewport-bug-workaround.js")
                .Include("~/wwwroot/lib/ui-select.js")
                .Include("~/wwwroot/lib/tinycolor.js")
                .Include("~/wwwroot/lib/angularjs-color-picker.js")
                .Include("~/wwwroot/lib/admin-lte-support.js")
                .Include("~/wwwroot/lib/ng-map.js")
                .Include("~/wwwroot/lib/ui-mask.js")
                .Include("~/wwwroot/lib/handsontable.full.js")
                .Include("~/wwwroot/lib/ngHandsontable.js")
                .Include("~/wwwroot/lib/file-saver.js")
                .Include("~/wwwroot/lib/ngGeolocation.js")
                .Include("~/wwwroot/lib/lodash.js")
                .Include("~/wwwroot/lib/angularjs-dropdown-multiselect.js")
                ;
            bundles.Add(scriptPublicLib);
            bundles.Add(new ScriptBundle("~/scripts/public/app/")
                //ui
                .Include("~/_public/ui/app.ui.js")
                .IncludeDirectory("~/_public/ui/directives", "*.js", true)
                .IncludeDirectory("~/_public/ui/filters", "*.js", true)
                .IncludeDirectory("~/_public/ui/services", "*.js", true)
                .IncludeDirectory("~/_public/ui/controllers", "*.js", true)
                //core
                .Include("~/_public/core/app.core.js")
                .IncludeDirectory("~/_public/core/directives", "*.js", true)
                .IncludeDirectory("~/_public/core/filters", "*.js", true)
                .IncludeDirectory("~/_public/core/services", "*.js", true)
                .IncludeDirectory("~/_public/core/controllers", "*.js", true)
                 //main
                 .Include("~/_public/main/app.main.js")
                .IncludeDirectory("~/_public/main/directives", "*.js", true)
                .IncludeDirectory("~/_public/main/filters", "*.js", true)
                .IncludeDirectory("~/_public/main/services", "*.js", true)
                .IncludeDirectory("~/_public/main/controllers", "*.js", true)
                //app
                .Include("~/_public/app.js"));

            var stylePublicLibLtr = new StyleBundle("~/wwwroot/public/css/lib/ltr").Include("~/wwwroot/css/*-ltr.css");
            stylePublicLibLtr.Orderer = OrderedBundler;
            bundles.Add(stylePublicLibLtr);
            var stylePublicLibRtl = new StyleBundle("~/wwwroot/public/css/lib/rtl").Include("~/wwwroot/css/*-rtl.css");
            stylePublicLibRtl.Orderer = OrderedBundler;
            bundles.Add(stylePublicLibRtl);

            var stylePublicLtr = new StyleBundle("~/wwwroot/public/css/ltr").Include("~/wwwroot/_public/css/*-ltr.css");
            stylePublicLtr.Orderer = OrderedBundler;
            bundles.Add(stylePublicLtr);
            var stylePublicRtl = new StyleBundle("~/wwwroot/public/css/rtl").Include("~/wwwroot/_public/css/*-rtl.css");
            stylePublicRtl.Orderer = OrderedBundler;
            bundles.Add(stylePublicRtl);
            /*************/

            var styleFront = new StyleBundle("~/wwwroot/front/css").Include("~/wwwroot/_front/css/*.css");
            styleFront.Orderer = OrderedBundler;
            bundles.Add(styleFront);

            ScriptBundle frontScripts = new ScriptBundle("~/wwwroot/front/scripts");
            frontScripts.Include("~/wwwroot/_front/js/*.js");
            frontScripts.Orderer = OrderedBundler;
            bundles.Add(frontScripts);


            var nonOrderedOrderer = new CustomBundleOrderer(false);

            foreach (Bundle bundle in bundles)
            {
                if (bundle.Orderer.GetType() != typeof(CustomBundleOrderer))
                {
                    bundle.Orderer = nonOrderedOrderer;
                }
            }

            //BundleTable.EnableOptimizations = true;
            
        }
    }
}
