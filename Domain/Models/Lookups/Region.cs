using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Lookups
{
    public class Region : BaseModel
    {
        public string Name { get; set; }
        public long Rank { get; set; }
        public bool IsActive { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }
    }
}
