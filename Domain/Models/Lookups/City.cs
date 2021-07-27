using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Lookups
{
    public class City : BaseModel
    {
        public string Name { get; set; }
        public long Rank { get; set; }
        public bool IsActive { get; set; }

        public long CountryId { get; set; }
        public virtual Country Country { get; set; }
        public ICollection<Region> Regions { get; set; }
    }
}
