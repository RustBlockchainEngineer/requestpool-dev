using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Recipient :BaseModel
    {
        public bool IsFailed{ get; set; }
        public string FailureReason { get; set; }
        public long ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public long? PublicUserId { get; set; }
        public virtual PublicUser PublicUser { get; set; }

        public long InvitationId { get; set; }
        public virtual Invitation Invitation { get; set; }

        public bool IsDraftResponse { get; set; }
        public DateTime? ResponseSubmitDate { get; set; }

        public virtual ICollection<ItemResponse> ItemsResponse{ get; set; }
        public virtual ICollection<RecipientResponseAttachment> Attachments{ get; set; }


    }
}
