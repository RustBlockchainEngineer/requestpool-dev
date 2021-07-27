using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Infrastructure.Mapping;
using Domain.Models;
using Domain.Models.Lookups;

namespace Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,long, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationDbContext(): base("DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = true;

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        //basic 
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<PublicUser> PublicUsers { get; set; }

        //lookups
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<EnquiryType> EnquiryTypes { get; set; }
        //
        public DbSet<Client> Clients { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Enquiry> Enquiries { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<EnquiryResponse> EnquiryResponses { get; set; }
        public DbSet<RecipientResponseAttachment> RecipientResponseAttachments { get; set; }
        public DbSet<ItemResponse> ItemResponses { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public DbSet<EnquiryAttachment> EnquiryAttachments { get; set; }
        public DbSet<MembershipPlan> MembershipPlans { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Settings> Settings { get; set; }

        public DbSet<Otp> Otp { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<ItemsDynamicProperty> ItemsDynamicProperties { get; set; }
        public DbSet<ItemDynamicPropertyResponse> ItemDynamicPropertyResponses { get; set; }
        public DbSet<ItemDynamicProperty> ItemDynamicProperties { get; set; }
        public DbSet<EnquiryItemsDynamicProperty> EnquiryItemsDynamicProperties { get; set; }

        public DbSet<VendorContactCategory> VendorContactCategories { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //
            base.OnModelCreating(modelBuilder);
            //
            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUsers");
            modelBuilder.Entity<ApplicationUserRole>().ToTable("ApplicationUserRoles");
            modelBuilder.Entity<ApplicationUserLogin>().ToTable("ApplicationUserLogins");
            modelBuilder.Entity<ApplicationUserClaim>().ToTable("ApplicationUserClaims");
            modelBuilder.Entity<ApplicationRole>().ToTable("ApplicationRoles");
        }
    }


}
