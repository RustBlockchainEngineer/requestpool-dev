using Domain.Models;
using Domain.Models.Lookups;
using Foolproof;
using Foundation.Areas.Public.Models.ViewModels;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Foundation.Areas.Public.Models
{
    public class EnquiryViewModel
    {
        public PublicUserBriefViewModel PublicUser { get; set; }

        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string PrNumber { get; set; }
        public string BoqNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int InvitationsCount { get; set; }
        public int ResponsesCount{ get; set; }
        public EnquiryTypeBriefViewModel EnquiryType { get; set; }
        public StatusBriefViewModel Status { get; set; }

        public ProjectBriefViewModel Project { get; set; }
        public ClientBriefViewModel Client { get; set; }
        public EnquiryBriefViewModel Parent { get; set; }

        public bool IsTemplate { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public EnquiryViewModel()
        {

        }
        public EnquiryViewModel(Enquiry model)
        {
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);

            Id = model.Id;
            Subject = model.Subject;
            ReferenceNumber = model.ReferenceNumber;
            RevisionNumber = model.RevisionNumber;
            PrNumber = model.PrNumber;
            BoqNumber = model.BoqNumber;
            Description = model.Description;
            Remarks = model.Remarks;
            IsTemplate = model.IsTemplate;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;
            if (model.Invitations != null)
            {
                InvitationsCount = model.Invitations.Count(i => !i.IsDeleted && !i.IsDraft);
                
                ResponsesCount = model.Invitations.Sum(i => i.Recipients.Count(r => !r.IsDeleted && !r.IsDraftResponse));
            }
            if(model.ProjectId.HasValue)
                Project = new ProjectBriefViewModel(model.Project);
            if (model.ClientId.HasValue)
                Client = new ClientBriefViewModel(model.Client);
            EnquiryType = new EnquiryTypeBriefViewModel(model.EnquiryType);
            if (model.ParentId.HasValue)
            {
                Parent = new EnquiryBriefViewModel(model.Parent);
            }
            if (model.StatusId.HasValue)
            {
                Status = new StatusBriefViewModel(model.Status);
            }
        }
    }

    public class EnquiryFormModel
    {
        public long? ProjectId { get; set; }
        public long? ClientId { get; set; }

        public long? StatusId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long EnquiryTypeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_title")]
        public string Subject { get; set; }

        [CustomRegularExpression(Validation.ReferenceNumber, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_reference_number")]
        public string ReferenceNumber { get; set; }

        [CustomRegularExpression(Validation.RevistionNumber, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_revision_number")]
        public string RevisionNumber { get; set; }

        [CustomRegularExpression(Validation.ReferenceNumber, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_reference_number")]
        public string PrNumber { get; set; }
        [CustomRegularExpression(Validation.ReferenceNumber, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_reference_number")]
        public string BoqNumber { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Description { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_notes")]
        public string Remarks { get; set; }

        public bool? IsTemplate { get; set; }

        public virtual void UpdateModel(Enquiry model)
        {
            model.ProjectId = ProjectId;
            model.ClientId = ClientId;
            model.EnquiryTypeId = EnquiryTypeId;
            model.Subject = Subject;
            model.ReferenceNumber = ReferenceNumber;
            model.RevisionNumber = RevisionNumber;
            model.PrNumber = PrNumber;
            model.BoqNumber = BoqNumber;
            model.Description = Description;
            model.Remarks = Remarks;
            model.StatusId = StatusId;
            if (IsTemplate.HasValue)
                model.IsTemplate = IsTemplate.Value;
            else model.IsTemplate = false;
        }
    }
    public class EnquiryPostModel : EnquiryFormModel
    {

    }

    public class EnquiryPutModel : EnquiryFormModel
    {

    }

    public class EnquiryCopyFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long EnquiryId { get; set; }
        
        [CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_title")]
        public string Subject { get; set; }

        [CustomRegularExpression(Validation.RevistionNumber, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_revision_number")]
        public string RevisionNumber { get; set; }

        public bool? IsTemplate { get; set; }
        public bool? IsRevision { get; set; }

    }
    public class EnquiryBriefViewModel
    {
        public long Id { get; set; }
        public string Subject { get; set; }
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public ProjectBriefViewModel Project { get; set; }
        public bool IsTemplate { get; set; }

        public EnquiryBriefViewModel() { }
        public EnquiryBriefViewModel(Enquiry model)
        {
            Id = model.Id;
            Subject = model.Subject;
            RevisionNumber = model.RevisionNumber;
            ReferenceNumber = model.ReferenceNumber;
            IsTemplate = model.IsTemplate;
            if (model.ProjectId.HasValue)
                Project = new ProjectBriefViewModel(model.Project);
        }

    }

    public class EnquirySearchModel
    {
        public string Username { get; set; }

        public string Subject { get; set; }
        public string ReferenceNumber { get; set; }
        public string PrNumber { get; set; }
        public string BoqNumber { get; set; }
        public long? EnquiryTypeId { get; set; }
        public long? StatusId { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsTemplate { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public EnquirySearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Subject == null)
                Subject = "";
            if (ReferenceNumber == null)
                ReferenceNumber = "";
            if (Username == null)
                Username = "";
            if (PrNumber == null)
                PrNumber = "";
            if (BoqNumber == null)
                BoqNumber = "";
            Search = i =>
                            (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            &&(!IsTemplate.HasValue || i.IsTemplate == IsTemplate.Value)
                            && (!EnquiryTypeId.HasValue || i.EnquiryTypeId == EnquiryTypeId.Value)
                            && (!StatusId.HasValue ||  i.StatusId == EnquiryTypeId.Value)
                            && (Subject == "" || i.Subject.ToLower().Contains(Subject))
                            && (ReferenceNumber == "" || (i.ReferenceNumber!= null && i.ReferenceNumber.Contains(ReferenceNumber)))
                            && (PrNumber == "" || (i.PrNumber != null && i.PrNumber.Contains(PrNumber)))
                            && (BoqNumber == "" || (i.BoqNumber != null && i.BoqNumber.Contains(BoqNumber)))
                            && (Username == "" || i.PublicUser.UserName.ToLower() == Username.ToLower().Trim());
        }
        public Func<Enquiry, bool> Search;
    }

    public class ReceivedEnquiryViewModel
    {
        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public StatusBriefViewModel Status { get; set; }

        public ReceivedEnquiryViewModel()
        {

        }
        public ReceivedEnquiryViewModel(Enquiry model)
        {
            Id = model.Id;
            Subject = model.Subject;
            ReferenceNumber = model.ReferenceNumber;
            RevisionNumber = model.RevisionNumber;
            Description = model.Description;
            if (model.StatusId.HasValue)
            {
                Status = new StatusBriefViewModel(model.Status);
            }
        }
    }

    public class EnquiryPreviewViewModel
    {
        public List<ItemResponseViewModel> Responses { get; set; }
        public List<ReceivedItemViewModel> Items { get; set; }

        public EnquiryPreviewViewModel()
        {

        }
        public EnquiryPreviewViewModel(IEnumerable<Item> items)
        {
            Items = items.Select(i => new ReceivedItemViewModel(i)).ToList();
        }
    }
}