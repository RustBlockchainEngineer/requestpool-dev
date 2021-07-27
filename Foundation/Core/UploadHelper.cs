using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace Foundation.Core
{
    public class UploadHelper
    {
        public static string GetPublicUserUrl(string fileName)
        {
            return AppSettings.Server + "/" + AppSettings.UploadFolder + "/public-user/" + fileName;
        }

        public static string GetPublicUserFolder()
        {
            return AppSettings.UploadFolder + "/public-user/";
        }

        
        public static string GetEnquiriesUrl(string fileName)
        {
            return AppSettings.Server + "/" + AppSettings.UploadFolder + "/enquiries/" + fileName;
        }

        public static string GetEnquiriesFolder()
        {
            return AppSettings.UploadFolder + "/enquiries/";
        }

        public static string GetGeneralUrl(string fileName)
        {
            return AppSettings.Server + "/" + AppSettings.UploadFolder + "/general/" + fileName;
        }

        public static string GetGeneralFolder()
        {
            return AppSettings.UploadFolder + "/general/";
        }

        public static string GetResponsesUrl(string fileName)
        {
            return AppSettings.Server + "/" + AppSettings.UploadFolder + "/responses/" + fileName;
        }

        public static string GetResponsessFolder()
        {
            return AppSettings.UploadFolder + "/responses/";
        }

        public static bool IsValidImage(string filename)
        {
            bool isValid = false;
            var extList = ".png,.jpg,.gif,.bmp,.jpeg".Split(',');
            string fileExt = Path.GetExtension(filename).ToLower();
            foreach(string ext in extList)
            {
                if(fileExt == ext.Trim())
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        public static bool IsValidFile(string filename)
        {
            bool isValid = false;
            var extList = ".png,.jpg,.gif,.bmp,.jpeg,.doc,.docx,.xls,.xlsx,.pdf,.txt,.zip".Split(',');
            string fileExt = Path.GetExtension(filename).ToLower();
            foreach (string ext in extList)
            {
                if (fileExt == ext.Trim())
                {
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }
    }
}