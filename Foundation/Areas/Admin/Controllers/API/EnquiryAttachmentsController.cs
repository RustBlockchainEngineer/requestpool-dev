using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Data.Entity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Mapster;
using Domain.Models.Lookups;
using Domain.Models;
using Foundation.Areas.Admin.Models;
using System.IO;
using Foundation.Areas.Public.Models;
using System.Data.Common;
using System.Data.SqlClient;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/enquiries-attachments")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]
    public class EnquiryAttachmentsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public EnquiryAttachmentsController()
        {
        }


        [ResponseType(typeof(ResponseMsg<IEnumerable<EnquiryAttachmentViewModel>>))]
        [HttpGet]
        [Route("{enquiryId}")]
        public async Task<HttpResponseMessage> Get(long enquiryId)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<IEnumerable<EnquiryAttachmentViewModel>> Msg = new ResponseMsg<IEnumerable<EnquiryAttachmentViewModel>>();
            Msg.Content = db.EnquiryAttachments
                .Where(i=> i.EnquiryId == enquiryId)
                .ToList()
                .Select(m => new EnquiryAttachmentViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
