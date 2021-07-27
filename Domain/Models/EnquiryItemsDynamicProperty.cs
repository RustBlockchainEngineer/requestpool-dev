using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EnquiryItemsDynamicProperty : BaseModel
    {
        public long EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }
        public long PropertyId { get; set; }
        public virtual ItemsDynamicProperty Property { get; set; }
        public int Rank { get; set; }

        public bool IsPublic { get; set; }
        public bool IsInfoOnly { get; set; }

        public EnquiryItemsDynamicProperty()
        {
            IsPublic = true;
        }
    }
}
