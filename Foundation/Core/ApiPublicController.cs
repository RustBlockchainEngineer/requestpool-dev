using MobileServies.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Foundation.Core
{
    //[EnableCors(origins: "*", headers: "*", methods: "GET, POST, PUT, DELETE, OPTIONS", SupportsCredentials = true)]
    [Authorize]
    public class ApiPublicController : BaseApiController
    {
       
    }
}