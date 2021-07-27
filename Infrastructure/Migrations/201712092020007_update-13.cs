namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "Latitude", c => c.String());
            AddColumn("dbo.ApplicationUsers", "Longitude", c => c.String());
            AddColumn("dbo.ApplicationUsers", "LatestIP", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "LatestIP");
            DropColumn("dbo.ApplicationUsers", "Longitude");
            DropColumn("dbo.ApplicationUsers", "Latitude");
        }
    }
}
