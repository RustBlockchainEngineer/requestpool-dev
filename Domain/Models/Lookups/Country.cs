using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Lookups
{
    public class Country: BaseModel
    {
        public string Name { get; set; }
        public long Rank { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}
