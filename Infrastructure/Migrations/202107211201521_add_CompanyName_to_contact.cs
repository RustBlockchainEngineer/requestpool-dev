namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_CompanyName_to_contact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contacts", "CompanyName");
        }
    }
}
