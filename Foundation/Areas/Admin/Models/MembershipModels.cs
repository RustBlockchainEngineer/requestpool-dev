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
    public class MembershipViewModel
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }

        public MembershipPlanBriefViewModel MembershipPlan { get; set; }
        public MembershipPlanBriefViewModel DowngradeToMembershipPlan { get; set; }
        public PublicUserBriefViewModel PublicUser { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public CreatorViewModel Creator { get; set; }
        public bool IsDeleted { get; set; }

        public MembershipViewModel()
        {

        }
        public MembershipViewModel(Membership model)
        {
            Id = model.Id;
            StartDate = model.StartDate;
            EndDate = model.EndDate;

            Remarks = model.Remarks;
            CreationDate = model.CreationDate;
            LastUpdateDate = model.LastUpdateDate;
            IsDeleted = model.IsDeleted;
            Creator = new CreatorViewModel()
            {
                Id = model.CreatorId,
                Name = model.Creator.Name,
                Username = model.Creator.UserName
            };
            PublicUser = new PublicUserBriefViewModel(model.PublicUser);
            MembershipPlan = new MembershipPlanBriefViewModel(model.MembershipPlan);
            if(model.DowngradeToMembershipPlanId.HasValue)
                DowngradeToMembershipPlan = new MembershipPlanBriefViewModel(model.DowngradeToMembershipPlan);
        }
    }

    public class MembershipFormModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long PublicUserId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public long MembershipPlanId { get; set; }

        public long? DowngradeToMembershipPlanId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Errors), ErrorMessageResourceName = "common_required")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual void UpdateModel(Membership model)
        {
            model.PublicUserId = PublicUserId;
            model.MembershipPlanId = MembershipPlanId;
            if(DowngradeToMembershipPlanId.HasValue)
                model.DowngradeToMembershipPlanId = DowngradeToMembershipPlanId;
            model.StartDate = StartDate;
            model.EndDate = EndDate;
        }
    }
    public class MembershipPostModel : MembershipFormModel
    {

    }

    public class MembershipPutModel : MembershipFormModel
    {

    }

    public class CurrentMembershipViewModel
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MembershipPlanBriefViewModel MembershipPlan { get; set; }
        public ConsumptionViewModel Consumption { get; set; }

        public CurrentMembershipViewModel()
        {

        }
        public CurrentMembershipViewModel(Membership model)
        {
            Id = model.Id;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            MembershipPlan = new MembershipPlanBriefViewModel(model.MembershipPlan);
        }
    }


    public class ConsumptionViewModel
    {
        public long EnquiriesPerMonth { get; set; }
        public long InvitationsPerMonth { get; set; }
        public long ItemFields { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}