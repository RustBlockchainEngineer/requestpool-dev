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

namespace Foundation.Areas.Public.Models
{
    public class MembershipViewModel
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public MembershipPlanBriefViewModel MembershipPlan { get; set; }

        public MembershipViewModel()
        {

        }
        public MembershipViewModel(Membership model)
        {
            Id = model.Id;
            StartDate = model.StartDate;
            EndDate = model.EndDate;
            MembershipPlan = new MembershipPlanBriefViewModel(model.MembershipPlan);
        }
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



}