using Domain.Models;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Foundation.Core
{
    public static class AppSettings
    {
        public static string PublicLoginHeader { get; set; }
        public static string AdminLoginHeader { get; set; }


        public static bool UseSMS { get; set; }
        public static bool UseDefaultEmail { get; set; }
        public static bool UseDefaultPhone { get; set; }

        public static string Server { get; set; }
        public static string UploadFolder { get; set; }
        public static string DefaultPhone { get; set; }
        public static string DefaultEmail { get; set; }
        public static int ItemsPerPage { get; set; }
        public static int MaxUploadSizeInKB { get; set; }
        // db settings
        public static Settings DbSettings { get; set; }


        public static void Load()
        {
            PublicLoginHeader = WebConfigurationManager.AppSettings["public-login-header"];
            AdminLoginHeader = WebConfigurationManager.AppSettings["admin-login-header"];

            Server = WebConfigurationManager.AppSettings["server"];
            UploadFolder = WebConfigurationManager.AppSettings["upload-folder"];
            DefaultPhone = WebConfigurationManager.AppSettings["default-phone"];
            DefaultEmail = WebConfigurationManager.AppSettings["default-email"];
            try
            {
                ItemsPerPage = Int32.Parse(WebConfigurationManager.AppSettings["items-per-page"]);
            }
            catch { ItemsPerPage = 15; }
            try
            {
                UseSMS = Boolean.Parse(WebConfigurationManager.AppSettings["use-sms"]);
            }
            catch { UseSMS = false; }

            try
            {
                UseDefaultEmail = Boolean.Parse(WebConfigurationManager.AppSettings["use-default-email"]);
            }
            catch { UseDefaultEmail = true; }
            try
            {
                UseDefaultPhone = Boolean.Parse(WebConfigurationManager.AppSettings["use-default-phone"]);
            }
            catch { UseDefaultPhone = true; }

            try
            {
                MaxUploadSizeInKB = Int32.Parse(WebConfigurationManager.AppSettings["max-upload-size-in-kb"]);
            }
            catch { MaxUploadSizeInKB = 5120; }

            ApplicationDbContext db = new ApplicationDbContext();
            DbSettings = db.Settings.FirstOrDefault();
            if(DbSettings == null)
            {
                DbSettings = new Settings() { DefaultMembershipPeriod = 0 };
            }
    }
}
}