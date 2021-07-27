namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update16 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "CompanyName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "CompanyName");
        }
    }
}
