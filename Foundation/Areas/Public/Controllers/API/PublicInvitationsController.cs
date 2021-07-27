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

namespace Foundation.Areas.Public.Controllers.API
{
    [RoutePrefix("api/invitations")]
    public class PublicInvitationsController : ApiPublicController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private PublicUserManager _userManager;
        private AdministratorUserManager _administratorUserManager;
        long _currentUserId = -1;
        string _currentUserName = "";
        public PublicInvitationsController()
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

        string currentUserName
        {
            get
            {
                if (_currentUserName == "")
                    _currentUserName = User.Identity.Name;
                return _currentUserName;
            }
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
        [Route("outgoing")]
        public HttpResponseMessage Outgoing(long enquiryId)
        {
            ResponseMsg<IEnumerable<InvitationViewModel>> Msg = new ResponseMsg<IEnumerable<InvitationViewModel>>();
             var list = db.Invitations
                .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.Enquiry)
               .Include(m => m.Recipients)
               .Where(i => i.EnquiryId == enquiryId && i.Enquiry.PublicUserId == currentUserId)
               .ToList();
            Msg.Content = list.Select(m => new InvitationViewModel(m));

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
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
               .Include(m => m.Enquiry)
               .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.Recipients)
               .Where(i => i.EnquiryId == enquiryId && i.Enquiry.PublicUserId == currentUserId)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
               .ToList().
               Select(m => new InvitationViewModel(m));
            Msg.TotalCount = db.Invitations
                .Where(i => i.EnquiryId == enquiryId && i.Enquiry.PublicUserId == currentUserId)
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
                .Include(i => i.Enquiry.PublicUser)
               .Include(m => m.Enquiry)
               .Include(m => m.Recipients.Select(i => i.Contact))
               .FirstOrDefault(i => i.Id == id && i.Enquiry.PublicUserId == currentUserId);
            if (item == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
           // db.Entry(item).Collection(i => i.Recipients).Load();
            Msg.Content = new InvitationViewModel(item);

            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ReceivedInvitationViewModel>>))]
        [HttpGet]
        [Route("incoming")]
        public HttpResponseMessage Incoming()
        {
            ResponseMsg<IEnumerable<ReceivedInvitationViewModel>> Msg = new ResponseMsg<IEnumerable<ReceivedInvitationViewModel>>();
            Msg.Content = db.Recipients
                .Include(i => i.PublicUser)
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => i.PublicUserId == currentUserId 
                    && !i.IsDeleted
                    && !i.IsFailed
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted)
                .ToList().Select(i => new ReceivedInvitationViewModel(i));

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
                .Where(i => i.PublicUserId == currentUserId
                    && !i.IsDeleted
                    && !i.IsFailed                    
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted)
               .Where(searchModel.Search)
               .OrderByDescending(i => i.Id)
               .Skip(searchModel.Skip.Value).Take(searchModel.ItemsPerPage.Value)
                .ToList().Select(i => new ReceivedInvitationViewModel(i));
            Msg.TotalCount = db.Recipients
                .Where(i => i.PublicUserId == currentUserId
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
            var recipient = db.Recipients
                .Include(i => i.PublicUser)
                .Include(i => i.Invitation.Enquiry)
                .Include(i => i.Invitation.Enquiry.PublicUser)
                .Where(i => i.InvitationId == id 
                    && i.PublicUserId == currentUserId
                    && !i.IsDeleted
                    && !i.IsFailed
                    && !i.Invitation.IsDeleted
                    && !i.Invitation.IsDraft
                    && !i.Invitation.Enquiry.IsDeleted)
                .FirstOrDefault();

            if (recipient == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            
            Msg.Content = new ReceivedInvitationViewModel(recipient);
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }

        [ResponseType(typeof(ResponseMsg<IEnumerable<ReceivedAttachmentViewModel>>))]
        [HttpGet]
        [Route("attachments/{invitationId}")]
        public async Task<HttpResponseMessage> Attachments(long invitationId)
        {
            var recipient = db.Recipients
                .Where(i => i.InvitationId == invitationId && i.PublicUserId == currentUserId)
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

        [ResponseType(typeof(ResponseMsg<InvitationViewModel>))]
        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> Post(InvitationPostModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if(model.EndDate.HasValue && model.EndDate < DateTime.UtcNow)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }

            var enquiry = db.Enquiries.FirstOrDefault(i => i.Id == model.EnquiryId && !i.IsTemplate
                       && i.PublicUserId == currentUserId);
            if (enquiry == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var invitationToSave = new Invitation();
            model.UpdateModel(invitationToSave);
            if(!invitationToSave.IsDraft)
            {
                invitationToSave.PostDate = DateTime.UtcNow;
            }
            invitationToSave.CreatorId = currentUserId;
            db.Invitations.Add(invitationToSave);
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            foreach(long contactId in model.Contacts)
            {
                bool isValidContact = db.Contacts.Count(i => i.Id == contactId && i.PublicUserId == currentUserId) > 0;
                if (isValidContact)
                {
                    var recipient = new Recipient();
                    recipient.ContactId = contactId;
                    recipient.IsFailed = false;
                    recipient.InvitationId = invitationToSave.Id;
                    recipient.IsDraftResponse = true;
                    recipient.CreatorId = currentUserId;
                    db.Recipients.Add(recipient);
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            if (!invitationToSave.IsDraft)
            {
                Send(invitationToSave.Id,invitationToSave);
            }
            return await OutgoingById(invitationToSave.Id);
        }

        [ResponseType(typeof(ResponseMsg<InvitationViewModel>))]
        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Put(long id, InvitationPutModel model)
        {
            if (!ModelState.IsValid)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (model.EndDate.HasValue && model.EndDate < DateTime.UtcNow)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var itemToSave = db.Invitations.FirstOrDefault(i => i.Id == id && model.EnquiryId == i.EnquiryId && !i.Enquiry.IsTemplate
                       && i.Enquiry.PublicUserId == currentUserId);
            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            var isOriginallyDraft = itemToSave.IsDraft;
            model.UpdateModel(itemToSave);
            if (!itemToSave.IsDraft && isOriginallyDraft)
            {
                itemToSave.PostDate = DateTime.UtcNow;
            }
            else
            {
                itemToSave.IsDraft = false;
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            if(isOriginallyDraft)
            {
                var recipientsToDelete = db.Recipients.Where(i => i.InvitationId == id);
                db.Recipients.RemoveRange(recipientsToDelete);
                foreach (long contactId in model.Contacts)
                {
                    bool isValidContact = db.Contacts.Count(i => i.Id == contactId && i.PublicUserId == currentUserId) > 0;
                    if (isValidContact)
                    {
                        var recipient = new Recipient();
                        recipient.ContactId = contactId;
                        recipient.IsFailed = false;
                        recipient.InvitationId = itemToSave.Id;
                        recipient.CreatorId = currentUserId;
                        db.Recipients.Add(recipient);
                    }
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception exp)
                {
                    ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                    ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                    return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
                }
                if (!itemToSave.IsDraft)
                {
                    Send(itemToSave.Id, itemToSave);
                }
            }
            return await OutgoingById(itemToSave.Id);
           
            
        }


        [ResponseType(typeof(ResponseMsg<string>))]
        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> Delete(long id)
        {
            var itemToSave = db.Invitations.FirstOrDefault(i => i.Id == id
            && i.Enquiry.PublicUserId == currentUserId);

            if (itemToSave == null)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_not_found;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            if (!itemToSave.IsDraft)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>();
                ErrorMsg.Message = Resources.Errors.http_bad_request;
                return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.BadRequest, ErrorMsg));
            }
            itemToSave.IsDeleted = true;
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
                ResponseMsg<string> ErrorMsg = new ResponseMsg<string>(ModelState);
                ErrorMsg.Message = Resources.Errors.http_internal_server_error;
                return Request.CreateResponse<ResponseMsg<String>>(HttpStatusCode.InternalServerError, ErrorMsg);
            }
            ResponseMsg<string> Msg = new ResponseMsg<string>();
            return Request.CreateResponse(HttpStatusCode.OK, Msg);
        }


        protected List<Recipient> Send(long invitationId,Invitation invitation)
        {
            var recipients = db.Recipients
                    .Include(i=> i.Contact)
                    //.Include(i=>i.PublicUser)
                    .Where(i => i.InvitationId == invitationId).ToList();
            var system = AdministratorUserManager.FindByName(SystemRoles.System);

            foreach (Recipient r in recipients)
            {
                Otp otp = null; 
                var user = PublicUserManager.FindByEmail(r.Contact.Email);
                if(user == null)
                {
                    otp = new Otp();
                    otp.CreationDate = DateTime.UtcNow;
                    otp.IP = Helper.GetClientIP();
                    otp.Code = Activation.CreateCode();
                    otp.Username = r.Contact.Email.Trim();
                    otp.Purpose = OtpPurposes.Login;
                    otp.CreatorId = system.Id;
                    db.Otp.Add(otp);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception exp)
                    {
                    }
                    //register user
                    user = new PublicUser();
                    user.UserName = user.Email =  r.Contact.Email;
                    user.Name = r.Contact.Name;
                    user.CreationDate = user.LastUpdateDate = DateTime.UtcNow;
                    user.CreatorId = currentUserId;
                    user.Otp = otp.Code;
                    user.IsActive = true;
                    IdentityResult result = PublicUserManager.Create(user, user.Otp);
                    if (!result.Succeeded)
                    {
                        r.IsFailed = true;
                        r.FailureReason = "System Error";
                        invitation.HasErrors = true;
                    }
                    else
                    {
                        PublicUserManager.AddToRole(user.Id, SystemRoles.PublicUser);
                        MembershipPlan defaultMembershipPlan = db.MembershipPlans.FirstOrDefault(m => m.IsActive && !m.IsDeleted && m.IsDefault);
                        if (defaultMembershipPlan != null)
                        {
                            Membership defaultMembership = new Membership();
                            defaultMembership.MembershipPlanId = defaultMembershipPlan.Id;
                            defaultMembership.PublicUserId = user.Id;
                            defaultMembership.StartDate = DateTime.UtcNow;
                            defaultMembership.EndDate = DateTime.UtcNow.AddDays(AppSettings.DbSettings.DefaultMembershipPeriod);
                            defaultMembership.CreatorId = system.Id;
                            db.Memberships.Add(defaultMembership);
                        }
                        try
                        {
                            db.SaveChanges();
                        }
                        catch
                        {
                            // fail silently
                        }
                    }
                }
                else if(user.IsDeleted || !user.IsActive)
                {
                    // set error message = locked by system
                    r.IsFailed = true;
                    r.FailureReason = "This email is locked by system";
                    invitation.HasErrors = true;
                }
                if (!r.IsFailed)
                {
                    r.PublicUserId = user.Id;
                    if (!SendInvitation(otp, user, invitation.Subject))
                    {
                        r.IsFailed = true;
                        r.FailureReason = "Failed to send email";
                        invitation.HasErrors = true;
                    }
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception exp)
            {
            }

            return recipients;
        }

       
        protected bool SendInvitation(Otp otp, PublicUser user,string subject)
        {
            bool result = false;
            string target = user.Email;

            string message = TemplatesManager.Render(TemplateKeys.Invitation,
                new InvitationTemplateModel()
                {
                    Name = user.Name,
                    Sender = currentUserName,
                    Subject = subject,
                    Url = AppSettings.Server,
                    Otp = otp !=null?otp.Code:null
                });
            if (MailService.Send(target,Resources.Mail.invitation_subject, message))
            {
                result = true;
                if (otp != null)
                {
                    otp.IsSent = true;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception exp)
                    {
                    }
                }

            }
           
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
