using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;

namespace Foundation.Core
{
    public class Activation
    {
        public static string CreateCode()
        {
            return new Random().Next(111111, 999999).ToString();
        }
    }
}