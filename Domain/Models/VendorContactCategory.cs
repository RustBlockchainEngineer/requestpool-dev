using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Lookups;

namespace Domain.Models
{
    public class VendorContactCategory : BaseModel
    {
        
        public long ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public long ContactTypeId { get; set; }
        public virtual ContactType ContactType { get; set; }

    }

}
