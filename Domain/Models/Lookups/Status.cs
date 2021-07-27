using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Lookups
{
    public class Status : BaseModel
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public long Rank { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
