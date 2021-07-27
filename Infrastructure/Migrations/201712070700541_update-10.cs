namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Settings", "DefaultMembershipPeriod", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Settings", "DefaultMembershipPeriod", c => c.Long(nullable: false));
        }
    }
}
