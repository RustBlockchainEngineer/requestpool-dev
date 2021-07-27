using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ItemsDynamicProperty :BaseModel
    {
        public string Name { get; set; }
        public long PropertyTypeId { get; set; }
        public virtual PropertyType PropertyType { get; set; }
        public long PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }

    }
}
