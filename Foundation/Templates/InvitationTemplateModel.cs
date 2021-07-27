using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foundation.Templates
{
    public class InvitationTemplateModel
    {
        public string Name { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string Otp { get; set; }
    }
}