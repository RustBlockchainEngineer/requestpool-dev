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
using System.IO;
using Foundation.Areas.Public.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/response-attachments/{invitationId}")]
    public class ResponseAttachmentsController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        long _currentUserId = -1;

        public ResponseAttachmentsController()
        {
        }

        long currentUserId
        {
            get
            {
                if (_currentUserId == -1)
                    _currentUserId = User.Identity.GetUserId<long>();
                return _currentUserId;
            }
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<RecipientResponseAttachmentViewModel>>))]
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> Get(long recipientId)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<IEnumerable<RecipientResponseAttachmentViewModel>> Msg = new ResponseMsg<IEnumerable<RecipientResponseAttachmentViewModel>>();
            Msg.Content = db.RecipientResponseAttachments
                .Where(m => m.Recipient.Id == recipientId
                            && !m.IsDeleted)
                .ToList()
                .Select(m => new RecipientResponseAttachmentViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
