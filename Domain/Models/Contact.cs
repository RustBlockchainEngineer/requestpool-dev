using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Contact :BaseModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }

        public long PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }

        public long ContactTypeId { get; set; }
        public virtual ContactType ContactType { get; set; }

    }
}
