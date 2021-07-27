using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Project :BaseModel
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        public long ClientId { get; set; }
        public virtual Client Client { get; set; }

        public virtual ICollection<Enquiry> Enquiries { get; set; }
    }
}
