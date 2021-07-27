using Domain.Models.Lookups;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PublicUser : ApplicationUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<PublicUser, long> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<PublicUser, long> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }

        public string Otp { get; set; }
        public string Photo { get; set; }
        public string Profile { get; set; }
        public string Remarks { get; set; }
        public string CompanyName { get; set; }

        public virtual ICollection<Client> Clients{ get; set; }
        public virtual ICollection<Enquiry> Enquiries{ get; set; }
        public virtual ICollection<Membership> Memberships{ get; set; }

    }
}
