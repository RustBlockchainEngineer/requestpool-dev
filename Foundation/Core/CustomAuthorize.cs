using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MobileServices.Core
{
    public class CustomAuthorize: System.Web.Http.AuthorizeAttribute
    {
        public CustomAuthorize(params string[] roles)
        {
            List<string> effectiveRoles = new List<string>(roles);
            effectiveRoles.Add(SystemRoles.System);
            effectiveRoles.Add(SystemRoles.Super);
            base.Roles = string.Join(",", effectiveRoles);
        }

        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
           if(actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                
            }
        }
    }
}