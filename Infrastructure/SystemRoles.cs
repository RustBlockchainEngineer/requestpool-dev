using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infrastructure
{
    public static class SystemRoles
    {
        public const string System = "system";
        public const string Super = "super";
        public const string Admin = "admin";
        public const string PublicUser = "public_user";
        public const string ManagePublicUsers = "manage_public_users";
        public const string ManageAdmins = "manage_admins";
        public const string ManageLookups = "manage_lookups";
        public const string ManageMemberships = "manage_membership";
        public const string ManageReports = "manage_reports";
        public const string ManageSettings = "manage_settings";

        public const string ViewContacts = "view_contacts";
        public const string ViewClients = "view_clients";
        public const string ViewProjects = "view_projects";
        public const string ViewEnquiries = "view_enquiries";
        public const string ViewInbox = "view_inbox";

    }
}