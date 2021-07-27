using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EnquiryResponse:BaseModel
    {
        
        public long RecipientId { get; set; }
        public virtual PublicUser Recipient { get; set; }

        public long EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }

    }
}
