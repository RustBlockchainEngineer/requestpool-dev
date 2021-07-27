using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PropertyType :BaseModel
    {
        public string Name { get; set; }
        public string UiRegex { get; set; }
        public string DbRegex { get; set; }
    }
}
