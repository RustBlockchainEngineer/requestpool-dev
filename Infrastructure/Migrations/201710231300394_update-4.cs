namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ItemDynamicPropertyResponses", "Value", c => c.String());
            DropColumn("dbo.ItemDynamicPropertyResponses", "Response");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ItemDynamicPropertyResponses", "Response", c => c.String());
            DropColumn("dbo.ItemDynamicPropertyResponses", "Value");
        }
    }
}
