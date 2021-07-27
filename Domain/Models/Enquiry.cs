using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Enquiry :BaseModel
    {
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string PrNumber { get; set; }
        public string BoqNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        public long EnquiryTypeId { get; set; }
        public virtual EnquiryType EnquiryType { get; set; }

        public long? ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public long? ClientId { get; set; }
        public virtual Client Client { get; set; }

        public long PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }

        public long? StatusId { get; set; }
        public virtual Status Status { get; set; }

        public ICollection<EnquiryAttachment> Attachments { get; set; }
        public ICollection<Invitation> Invitations { get; set; }

        public ICollection<Item> Items { get; set; }

        public long? ParentId { get; set; }
        public virtual Enquiry Parent { get; set; }

        public bool IsTemplate { get; set; }
    }
}
