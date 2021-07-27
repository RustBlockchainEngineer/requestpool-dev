using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Invitation :BaseModel
    {
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateTime? PostDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDraft { get; set; }
        public bool HasErrors { get; set; }

        public long EnquiryId { get; set; }
        public virtual Enquiry Enquiry { get; set; }

        public ICollection<Recipient> Recipients { get; set; }
    }
}
