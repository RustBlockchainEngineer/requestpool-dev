using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class MembershipPlan :BaseModel
    {
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
        public bool IsDefault { get; set; }
        public bool IsDefaultDowngrade { get; set; }
        public bool IsActive { get; set; }

    }
}
