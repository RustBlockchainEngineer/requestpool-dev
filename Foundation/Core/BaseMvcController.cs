using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Foundation.Core
{
    public class BaseMvcController : Controller
    {
        protected ActionResult RedirectToLocal(string returnUrl,string otherwiseAction="Index", string otherwiseController="Home")
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(otherwiseAction, otherwiseController);
        }
    }
}