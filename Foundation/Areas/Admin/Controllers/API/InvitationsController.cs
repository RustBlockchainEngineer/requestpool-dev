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
using Foundation.Templates;
using MobileServices.Core;

namespace Foundation.Areas.Admin.Controllers.API
{
    [RoutePrefix("admin/api/invitations")]
    [CustomAuthorize(SystemRoles.ViewEnquiries)]
    public class InvitationsController : ApiAdminController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private PublicUserManager _userManager;
        private AdministratorUserManager _administratorUserManager;
        public InvitationsController()
        {
        }

        public PublicUserManager PublicUserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<PublicUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public AdministratorUserManager AdministratorUserManager
        {
            get
            {
                return _administratorUserManager ?? Request.GetOwinContext().GetUserManager<AdministratorUserManager>();
            }
            private set
            {
                _administratorUserManager = value;
            }
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<InvitationViewModel>>))]
        [HttpGet]
        [Route("outgoing/search")]
        public HttpResponseMessage OutgoingSearch([FromUri]long enquiryId, [FromUri]InvitationSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new InvitationSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<InvitationViewModel>> Msg = new ResponseMsg<IEnumerable<InvitationViewModel>>();

            Msg.Content = db.Invitations
                .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.Enquiry)
               .Include(m => m.Recipients)
               .Where(i => i.EnquiryId == enquiryId)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new InvitationViewModel(m));
            Msg.TotalCount = db.Invitations
                .Where(i => i.EnquiryId == enquiryId)
                .Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<InvitationViewModel>))]
        [HttpGet]
        [Route("outgoing/{id}")]
        public async Task<HttpResponseMessage> OutgoingById(long id)
        {
            ResponseMsg<InvitationViewModel> Msg = new ResponseMsg<InvitationViewModel>();
            var item = db.Invitations
               .Include(m => m.Enquiry)
               .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.Recipients.Select(i => i.Contact))
               .FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            Msg.Content = new InvitationViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<InvitationViewModel>>))]
        [HttpGet]
        [Route("incoming/search")]
        public HttpResponseMessage IncomingSearch([FromUri]ReceivedInvitationSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new ReceivedInvitationSearchModel();
            searchModel.Init();
            ResponseMsg<IEnumerable<ReceivedInvitationViewModel>> Msg = new ResponseMsg<IEnumerable<ReceivedInvitationViewModel>>();
            Msg.Content = db.Recipients
                .Include(i => i.PublicUser)
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => !i.IsDeleted
                    && !i.IsFailed
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted)
                .Where(i => i.PublicUserId.HasValue)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
                .ToList().Select(i => new ReceivedInvitationViewModel(i));
            Msg.TotalCount = db.Recipients                
                .Where(i => i.PublicUserId.HasValue
                    && !i.IsDeleted
                    && !i.IsFailed
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted)
                .Where(searchModel.Search).Count();

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<ReceivedInvitationViewModel>))]
        [HttpGet]
        [Route("incoming/{id}")]
        public async Task<HttpResponseMessage> IncomingById(long id)
        {

            ResponseMsg<ReceivedInvitationViewModel> Msg = new ResponseMsg<ReceivedInvitationViewModel>();
            var item = db.Recipients
                .Include(i => i.PublicUser)
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => i.Id == id
                    && !i.IsDeleted
                    && !i.IsFailed
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted)
                .FirstOrDefault();

            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            Msg.Content = new ReceivedInvitationViewModel(item);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ReceivedAttachmentViewModel>>))]
        [HttpGet]
        [Route("attachments/{invitationId}")]
        public async Task<HttpResponseMessage> Attachments(long invitationId)
        {
            var recipient = db.Recipients
                .Where(i => i.InvitationId == invitationId)
                .FirstOrDefault();

            if (recipient == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            ResponseMsg<IEnumerable<ReceivedAttachmentViewModel>> Msg = new ResponseMsg<IEnumerable<ReceivedAttachmentViewModel>>();
            Msg.Content = db.EnquiryAttachments
                .Where(m => m.EnquiryId == recipient.Invitation.EnquiryId && !m.IsDeleted)
                .ToList()
                .Select(m => new ReceivedAttachmentViewModel(m));
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
