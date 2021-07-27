namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EnquiryItemsDynamicProperties", "IsPublic", c => c.Boolean(nullable: false));
            DropColumn("dbo.EnquiryItemsDynamicProperties", "IsVisible");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EnquiryItemsDynamicProperties", "IsVisible", c => c.Boolean(nullable: false));
            DropColumn("dbo.EnquiryItemsDynamicProperties", "IsPublic");
        }
    }
}
