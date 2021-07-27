using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BaseModel
    {
        public long Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public long CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public bool IsDeleted { get; set; }
        public BaseModel()
        {
            CreationDate = LastUpdateDate = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
}
