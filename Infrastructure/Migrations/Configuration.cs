namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Infrastructure.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "RequestManager";
        }

        protected override void Seed(Infrastructure.ApplicationDbContext context)
        {
            Seeds.Roles(context);
            Seeds.Users(context);
            Seeds.PropertyTypes(context);
            Seeds.Settings(context);
            Seeds.MembershipPlans(context);
            Seeds.Status(context);
        }
    }
}
