using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ItemResponse:BaseModel
    {
        public string Response { get; set; }

        public long RecipientId { get; set; }
        public virtual Recipient Recipient { get; set; }

        public long ItemId { get; set; }
        public virtual Item Item { get; set; }

        public virtual ICollection<ItemDynamicPropertyResponse> DynamicPropertiesResponses { get; set; }

    }
}
