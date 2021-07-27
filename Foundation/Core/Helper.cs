using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;

namespace Foundation.Core
{
    public static class Helper
    {
        public static string GetClientIP()
        {

            string Ret = "";
            //X-Forwarded-For: client1, proxy1, proxy2, ...
            string xff = HttpContext.Current.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(xff))
            {
                int idx = xff.IndexOf(',');
                if (0 < idx)
                    Ret = xff.Substring(0, idx);
                else
                    Ret = xff;
            }
            if (String.IsNullOrEmpty(Ret))
            {
                Ret = HttpContext.Current.Request.UserHostAddress;
            }
            return Ret;
        }


        
    }
}