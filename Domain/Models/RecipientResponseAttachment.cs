using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class RecipientResponseAttachment :BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string OriginalFileName { get; set; }

        public long RecipientId { get; set; }
        public virtual Recipient Recipient { get; set; }
    }
}
