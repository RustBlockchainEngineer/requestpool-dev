using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Client :BaseModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }

        public long PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }
}
