using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Membership :BaseModel
    {

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }

        public long MembershipPlanId { get; set; }
        public virtual MembershipPlan MembershipPlan { get; set; }

        public long? DowngradeToMembershipPlanId { get; set; }
        public virtual MembershipPlan DowngradeToMembershipPlan { get; set; }

        public long PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }
        
    }
}
