using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Lookups
{
    public class ContactType : BaseModel
    {
        public string Name { get; set; }
        public long PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }
    }
}
