using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class EnquiryAttachment :BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public long EnquiryId { get; set; }
        public virtual Enquiry Enquiry{ get; set; }
    }
}
