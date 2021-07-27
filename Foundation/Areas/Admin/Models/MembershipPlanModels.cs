using Domain.Models;
using Foolproof;
using Foundation.Areas.Admin.Models.ViewModels;
using Foundation.Core;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Foundation.Areas.Admin.Models
{
    public class MembershipPlanViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long MaxEnquiriesPerMonth { get; set; }
        //public long MaxRequestsPerEnquiry { get; set; }
        public long MaxItemsPerEnquiry { get; set; }
        public long MaxInvitationsPerMonth { get; set; }
        public long MaxContactsPerInvitation { get; set; }
        //public long MaxEnquiryFields { get; set; }
        //public long MaxRequestFields { get; set; }
        public long MaxItemFields { get; set; }
        public decimal CostPerMonth { get; set; }
        public decimal CostPerYear { get; set; }
        public string Remarks { get; set; }

        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDefaultDowngrade { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public CreatorViewModel Creator { get; set; }
        public bool IsDeleted { get; set; }

        public MembershipPlanViewModel()
        {

        }
        public MembershipPlanViewModel(MembershipPlan model)
        {
            Id = model.Id;
            Title = model.Title;
            Description = model.Description;
            MaxEnquiriesPerMonth = model.MaxEnquiriesPerMonth;
            //MaxRequestsPerEnquiry = model.MaxRequestsPerEnquiry;
            MaxItemsPerEnquiry = model.MaxItemsPerEnquiry;
            MaxInvitationsPerMonth = model.MaxInvitationsPerMonth;
            MaxContactsPerInvitation = model.MaxContactsPerInvitation;
            //MaxEnquiryFields = model.MaxEnquiryFields;
            //MaxRequestFields = model.MaxRequestFields;
            MaxItemFields = model.MaxItemFields;
            CostPerMonth = model.CostPerMonth;
            CostPerYear = model.CostPerYear;
            Remarks = model.Remarks;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsPublic = model.IsPublic;
            IsActive = model.IsActive;
            IsDefault = model.IsDefault;
            IsDefaultDowngrade = model.IsDefaultDowngrade;
            IsDeleted = model.IsDeleted;
            Creator = new CreatorViewModel()
            {
                Id = model.CreatorId,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };
        }
    }

    public class MembershipPlanFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_required")]
        [CustomRegularExpression(Validation.Title, ErrorMessageResourceType = typeof(Resources.Errors),
                    ErrorMessageResourceName = "common_title")]
        public string Title { get; set; }

        [CustomRegularExpression(Validation.Description, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_description")]
        public string Description { get; set; }

        public long MaxEnquiriesPerMonth { get; set; }
        //public long MaxRequestsPerEnquiry { get; set; }
        public long MaxItemsPerEnquiry { get; set; }
        public long MaxInvitationsPerMonth { get; set; }
        public long MaxContactsPerInvitation { get; set; }
        //public long MaxEnquiryFields { get; set; }
        //public long MaxRequestFields { get; set; }
        public long MaxItemFields { get; set; }
        public decimal CostPerMonth { get; set; }
        public decimal CostPerYear { get; set; }

        [CustomRegularExpression(Validation.Notes, ErrorMessageResourceType = typeof(Resources.Errors),
            ErrorMessageResourceName = "common_notes")]
        public string Remarks { get; set; }

        public bool IsPublic { get; set; }
        public bool IsActive { get; set; }

        public void UpdateModel(MembershipPlan model)
        {
            model.Title = Title;
            model.Description = Description;
            model.Remarks = Remarks;
            model.MaxEnquiriesPerMonth = MaxEnquiriesPerMonth;
            //model.MaxRequestsPerEnquiry = MaxRequestsPerEnquiry;
            model.MaxItemsPerEnquiry = MaxItemsPerEnquiry;
            model.MaxInvitationsPerMonth = MaxInvitationsPerMonth;
            model.MaxContactsPerInvitation = MaxContactsPerInvitation;
            //model.MaxEnquiryFields = MaxEnquiryFields;
            //model.MaxRequestFields = MaxRequestFields;
            model.MaxItemFields = MaxItemFields;
            model.CostPerMonth = CostPerMonth;
            model.CostPerYear = CostPerYear;
            model.IsPublic = IsPublic;
            model.IsActive = IsActive;
        }
    }
    public class MembershipPlanPostModel : MembershipPlanFormModel
    {

    }

    public class MembershipPlanPutModel : MembershipPlanFormModel
    {

    }

    public class MembershipPlanBriefViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long MaxEnquiriesPerMonth { get; set; }
        public long MaxItemsPerEnquiry { get; set; }
        public long MaxInvitationsPerMonth { get; set; }
        public long MaxContactsPerInvitation { get; set; }
        public long MaxItemFields { get; set; }
        public decimal CostPerMonth { get; set; }
        public decimal CostPerYear { get; set; }
        public MembershipPlanBriefViewModel() { }
        public MembershipPlanBriefViewModel(MembershipPlan model)
        {
            Id = model.Id;
            Title = model.Title;
            Description = model.Description;
            MaxEnquiriesPerMonth = model.MaxEnquiriesPerMonth;
            MaxItemsPerEnquiry = model.MaxItemsPerEnquiry;
            MaxInvitationsPerMonth = model.MaxInvitationsPerMonth;
            MaxContactsPerInvitation = model.MaxContactsPerInvitation;
            MaxItemFields = model.MaxItemFields;
            CostPerMonth = model.CostPerMonth;
            CostPerYear = model.CostPerYear;
        }
    }

    public class MembershipPlanSearchModel
    {
        public string Title { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public int? PageNumber { get; set; }
        public int? ItemsPerPage { get; set; }

        public int? Skip { get; set; }

        public MembershipPlanSearchModel() { }
        public void Init()
        {
            if (!PageNumber.HasValue)
                PageNumber = 1;
            if (!ItemsPerPage.HasValue)
                ItemsPerPage = AppSettings.ItemsPerPage;
            Skip = (PageNumber.Value - 1) * ItemsPerPage.Value;
            if (Title == null)
                Title = "";
            Search = i =>
                            (!IsPublic.HasValue || i.IsPublic == IsPublic.Value)
                            && (!IsActive.HasValue || i.IsActive == IsActive.Value)
                            && (!IsDeleted.HasValue || i.IsDeleted == IsDeleted.Value)
                            && (Title == "" || i.Title.ToLower().Contains(Title.ToLower()));
        }
        public Func<MembershipPlan, bool> Search;
    }

}