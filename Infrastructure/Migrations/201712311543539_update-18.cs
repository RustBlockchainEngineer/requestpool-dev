namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update18 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enquiries", "PrNumber", c => c.String());
            AddColumn("dbo.Enquiries", "BoqNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Enquiries", "BoqNumber");
            DropColumn("dbo.Enquiries", "PrNumber");
        }
    }
}
