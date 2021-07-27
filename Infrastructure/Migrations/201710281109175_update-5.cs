namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Invitations", "EndDate", c => c.DateTime());
            AddColumn("dbo.EnquiryItemsDynamicProperties", "IsInfoOnly", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EnquiryItemsDynamicProperties", "IsInfoOnly");
            DropColumn("dbo.Invitations", "EndDate");
        }
    }
}
