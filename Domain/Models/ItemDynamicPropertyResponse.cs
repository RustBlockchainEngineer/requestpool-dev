using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ItemDynamicPropertyResponse :BaseModel
    {
        public long ItemResponseId { get; set; }
        public virtual ItemResponse ItemResponse { get; set; }
        public long PropertyId { get; set; }
        public virtual ItemsDynamicProperty Property { get; set; }
        public string Value { get; set; }
    }
}
