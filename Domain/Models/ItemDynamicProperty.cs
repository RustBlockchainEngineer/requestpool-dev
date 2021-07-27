using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ItemDynamicProperty :BaseModel
    {
        public long ItemId { get; set; }
        public virtual Item Item { get; set; }
        public long DynamicPropertyId { get; set; }
        public virtual ItemsDynamicProperty DynamicProperty { get; set; }
        public string Value { get; set; }
        public bool IsApplicable { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsRequired { get; set; }
    }
}
