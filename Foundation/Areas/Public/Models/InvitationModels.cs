using Domain.Models;
using Domain.Models.Lookups;
using Foolproof;
using Foundation.Areas.Public.Models.ViewModels;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Foundation.Areas.Public.Models
{
    public class InvitationViewModel
    {
        public long Id { get; set; }
        public PublicUserBriefViewModel PublicUser { get; set; }

        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDraft { get; set; }
        public bool HasErrors { get; set; }

        public EnquiryBriefViewModel Enquiry { get; set; }
        public List<RecipientBriefViewModel> Recipients { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public InvitationViewModel()
        {

        }
        public InvitationViewModel(Invitation model)
        {
            PublicUser = new PublicUserBriefViewModel(model.Enquiry.PublicUser);
            Id = model.Id;
            Subject = model.Subject;
            Description = model.Description;
            PostDate = model.PostDate;
            EndDate = model.EndDate;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;
            IsDraft = model.IsDraft;
            HasErrors = model.HasErrors;
            Enquiry = new EnquiryBriefViewModel(model.Enquiry);
            Recipients = new List<RecipientBriefViewModel>();
            if(model.Recipients != null)
            {
                foreach(Recipient r in model.Recipients)
                {
                    Recipients.Add(new RecipientBriefViewModel(r));
                }
            }
        }
    }

    public class InvitationFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long EnquiryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_title")]
        public string Subject { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public bool IsDraft { get; set; }

        public List<long> Contacts { get; set; }





        public virtual void UpdateModel(Invitation model)
        {
            model.EnquiryId = EnquiryId;
            model.Subject = Subject;
            model.Description = Description;
            model.IsDraft = IsDraft;
            model.EndDate = EndDate;
        }
    }
    public class InvitationPostModel : InvitationFormModel
    {

    }

    public class InvitationPutModel : InvitationFormModel
    {

    }
    public class InvitationBriefViewModel
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? EndDate { get; set; }

        public InvitationBriefViewModel() { }
        public InvitationBriefViewModel(Invitation model)
        {
            Id = model.Id;
            Subject = model.Subject;
            PostDate = model.PostDate;
            EndDate = model.EndDate;
        }

    }
    public class ReceivedInvitationViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }
        public long Id { get; set; }
        public long RecipientId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDraftResponse { get; set; }
        public DateTime? ResponseSubmitDate { get; set; }
        public bool IsSaveEnabled { get; set; }


        public SenderViewModel Sender { get; set; }

        public ReceivedEnquiryViewModel Enquiry { get; set; }

        public ReceivedInvitationViewModel()
        {

        }
        public ReceivedInvitationViewModel(Recipient model)
        {
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);
            RecipientId = model.Id;
            Id = model.InvitationId;
            Subject = model.Invitation.Subject;
            Description = model.Invitation.Description;
            PostDate = model.Invitation.PostDate;
            EndDate = model.Invitation.EndDate;
            Enquiry = new ReceivedEnquiryViewModel(model.Invitation.Enquiry);
            Sender = new SenderViewModel(model.Invitation.Enquiry.PublicUser);
            IsDraftResponse = model.IsDraftResponse;
            ResponseSubmitDate = model.ResponseSubmitDate;
            IsSaveEnabled = (!EndDate.HasValue || (EndDate.Value - DateTime.UtcNow).Days >= 0) && Enquiry.Status.Id == (long)SystemStatus.Open;
        }
    }

    public class ReceivedAttachmentViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }

        public ReceivedAttachmentViewModel()
        {

        }
        public ReceivedAttachmentViewModel(EnquiryAttachment model)
        {
            Title = model.Title;
            Description = model.Description;
            Filename = model.FileName;
            Url = UploadHelper.GetEnquiriesUrl(Filename);
        }
    }

    public class InvitationSearchModel
    {
        public string Username { get; set; }

        public string Subject { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? EnquiryId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsDraft { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public InvitationSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Subject == null)
                Subject = "";
            if (Username == null)
                Username = "";
            Search = i =>
                            (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (!IsDraft.HasValue || i.IsDraft == IsDraft.Value)
                            && (!EnquiryId.HasValue || i.EnquiryId == EnquiryId.Value)
                            && (!StartDate.HasValue || DbFunctions.DiffDays(StartDate, i.PostDate) >= 0)
                            && (!EndDate.HasValue || DbFunctions.DiffDays(i.PostDate, EndDate) > 0)
                            && (Subject == "" || i.Subject.ToLower().Contains(Subject))
                            && (Username == "" || i.Enquiry.PublicUser.UserName.ToLower() == Username.ToLower().Trim());

        }
        public Func<Invitation, bool> Search;
    }

    public class ReceivedInvitationSearchModel
    {
        public string Username { get; set; }

        public string Subject { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsDraftResponse { get; set; }


        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public ReceivedInvitationSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Subject == null)
                Subject = "";
            if (email == null)
                email = "";
            if (name == null)
                name = "";
            if (Username == null)
                Username = "";
            Search = i =>
                            (!IsDraftResponse.HasValue || i.IsDraftResponse == IsDraftResponse.Value)
                            && (!StartDate.HasValue || DbFunctions.DiffDays(StartDate, i.Invitation.PostDate) >= 0)
                            && (!EndDate.HasValue || DbFunctions.DiffDays(i.Invitation.PostDate, EndDate) > 0)
                            && (Subject == "" || i.Invitation.Subject.ToLower().Contains(Subject))
                            && (email == "" || i.Invitation.Enquiry.Project.Client.PublicUser.UserName.ToLower().Contains(email))
                            && (name == "" || i.Invitation.Enquiry.PublicUser.Name.ToLower().Contains(name))
                            && (Username == "" || i.PublicUser.UserName.ToLower() == Username.ToLower().Trim());
        }
        public Func<Recipient, bool> Search;
    }
}