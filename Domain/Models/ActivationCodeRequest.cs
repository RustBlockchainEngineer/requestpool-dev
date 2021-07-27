using Domain.Models.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Otp : BaseModel
    {
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Purpose { get; set; }
        public string IP { get; set; }
        public string Code { get; set; }
        public Boolean IsUsed { get; set; }
        public Boolean IsSent { get; set; }
        public Boolean IsSetByAdmin { get; set; }
        public DateTime? UseDate { get; set; }
        
    }
}
