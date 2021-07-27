using Domain.Models;
using Domain.Models.Lookups;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Infrastructure
{
    public class Seeds
    {
        public static void Roles(ApplicationDbContext context)
        {
            var roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));
            if (!context.Roles.Any(r => r.Name == SystemRoles.System))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.System });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.Super))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.Super });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.Admin))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.Admin });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.PublicUser))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.PublicUser });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ManageAdmins))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ManageAdmins });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ManagePublicUsers))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ManagePublicUsers });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ManageMemberships))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ManageMemberships });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ManageLookups))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ManageLookups });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ManageSettings))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ManageSettings });
            }

           
            if (!context.Roles.Any(r => r.Name == SystemRoles.ViewContacts))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ViewContacts });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ViewClients))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ViewClients });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ViewProjects))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ViewProjects });
            }
            if (!context.Roles.Any(r => r.Name == SystemRoles.ViewEnquiries))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ViewEnquiries });
            }
           
            if (!context.Roles.Any(r => r.Name == SystemRoles.ViewInbox))
            {
                roleManager.Create(new ApplicationRole { Name = SystemRoles.ViewInbox });
            }
        }

        public static void Users(ApplicationDbContext context)
        {
            var userManager = new AdministratorUserManager(new AdministratorUserStore(context));
            var publicUserManager = new PublicUserManager(new PublicUserStore(context));

            var systemUser = userManager.FindByName("system");
            if (systemUser == null)
            {
                systemUser = new Administrator
                {
                    Name = "system",
                    UserName = "system",
                    CreationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    IsActive = true
                };
                userManager.Create(systemUser, "system");
                userManager.AddToRole(systemUser.Id, SystemRoles.System);
            }

            if (!context.Administrators.Any(u => u.UserName == "super"))
            {
                var user = new Administrator
                {
                    Name = "super",
                    UserName = "super",
                    CreationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatorId = systemUser.Id

                };
                userManager.Create(user, "super");
                userManager.AddToRole(user.Id, SystemRoles.Super);
            }

            if (!context.Administrators.Any(u => u.UserName == "admin"))
            {
                var user = new Administrator
                {
                    Name = "admin",
                    UserName = "admin",
                    CreationDate = DateTime.UtcNow,
                    LastUpdateDate = DateTime.UtcNow,
                    IsActive = true,
                    CreatorId = systemUser.Id

                };
                userManager.Create(user, "admin");
                userManager.AddToRole(user.Id, SystemRoles.Admin);
            }
        }

        public static void PropertyTypes(ApplicationDbContext context)
        {
            var userManager = new AdministratorUserManager(new AdministratorUserStore(context));

            if (context.PropertyTypes.Count() == 0)
            {
                var systemUser = userManager.FindByName("system");

                var text = new PropertyType() {
                    Name = FieldTypes.Text,
                    UiRegex="",
                    DbRegex="",
                    CreatorId = systemUser.Id
                };
                var number = new PropertyType()
                {
                    Name = FieldTypes.Number,
                    UiRegex = @"(?=^\-?[0 - 9] *\.?[0 - 9] +$|^$)",
                    DbRegex = "",
                    CreatorId = systemUser.Id

                };
                context.PropertyTypes.Add(text);
                context.PropertyTypes.Add(number);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception exp)
                {
                }
            }
        }

        public static void Settings(ApplicationDbContext context)
        {
            var userManager = new AdministratorUserManager(new AdministratorUserStore(context));
            var systemUser = userManager.FindByName("system");

            if (context.Settings.Count() == 0 )
            {
                var itemToSave = new Settings();
                itemToSave.CreatorId = systemUser.Id;
                itemToSave.DefaultMembershipPeriod = 90;
                context.Settings.Add(itemToSave);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception exp)
                {
                }
            }
        }

        public static void MembershipPlans(ApplicationDbContext context)
        {
            var userManager = new AdministratorUserManager(new AdministratorUserStore(context));
            var systemUser = userManager.FindByName("system");

            if (context.MembershipPlans.Count(m=>m.IsDefault && !m.IsDeleted && m.IsActive) == 0)
            {
                var itemToSave = new MembershipPlan();
                itemToSave.CreatorId = systemUser.Id;
                itemToSave.IsActive = true;
                itemToSave.IsDefault = true;
                itemToSave.IsPublic = false;
                itemToSave.MaxEnquiriesPerMonth = 1000;
                itemToSave.MaxInvitationsPerMonth = 1000;
                itemToSave.MaxContactsPerInvitation = 1000;
                itemToSave.MaxItemFields = 1000;
                itemToSave.MaxItemsPerEnquiry = 1000;
                itemToSave.CostPerMonth = 0;
                itemToSave.CostPerYear = 0;
                context.MembershipPlans.Add(itemToSave);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception exp)
                {
                }
            }
        }

        public static void Status(ApplicationDbContext context)
        {
            var userManager = new AdministratorUserManager(new AdministratorUserStore(context));
            var systemUser = userManager.FindByName("system");

            if (context.Status.Count() == 0)
            {
                string sql = @" SET IDENTITY_INSERT Status ON;
                                INSERT INTO Status(ID,Name,IsActive,IsDeleted,IsDefault,Rank,CreationDate, LastUpdateDate,CreatorId) VALUES (10,'Open', 1,0,0,0, GETUTCDATE(),GETUTCDATE(),@CreatorId);
                                INSERT INTO Status(ID,Name,IsActive,IsDeleted,IsDefault,Rank,CreationDate, LastUpdateDate,CreatorId) VALUES (20,'Closed', 1,0,0,0, GETUTCDATE(),GETUTCDATE(),@CreatorId);
                                INSERT INTO Status(ID,Name,IsActive,IsDeleted,IsDefault,Rank,CreationDate, LastUpdateDate,CreatorId) VALUES (30,'Cancelled', 1,0,0,0, GETUTCDATE(),GETUTCDATE(),@CreatorId);
                                INSERT INTO Status(ID,Name,IsActive,IsDeleted,IsDefault,Rank,CreationDate, LastUpdateDate,CreatorId) VALUES (40,'On Hold', 1,0,0,0, GETUTCDATE(),GETUTCDATE(),@CreatorId);
                                SET IDENTITY_INSERT Status OFF;
                                UPDATE Enquiries SET StatusId = 10";

                try
                {
                    context.Database.ExecuteSqlCommand(sql, new SqlParameter("@CreatorId", systemUser.Id));
                }
                catch (Exception exp)
                {
                }
            }
        }

    }
}
