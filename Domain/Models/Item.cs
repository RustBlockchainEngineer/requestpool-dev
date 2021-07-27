using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Item :BaseModel
    {
        public string ReferenceNumber { get; set; }
        public string RevisionNumber { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        public long? ItemTypeId { get; set; }
        public virtual ItemType ItemType { get; set; }

        public long EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }

        public virtual ICollection<ItemResponse> Responses { get; set; }
        public virtual ICollection<ItemDynamicProperty> DynamicProperties { get; set; }

        public long? CopiedFromId { get; set; }
        //public virtual Item CopiedFrom { get; set; }

    }
}
