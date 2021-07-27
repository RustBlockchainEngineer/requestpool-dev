namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MembershipPlans", "IsDefaultDowngrade", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MembershipPlans", "IsDefaultDowngrade");
        }
    }
}
