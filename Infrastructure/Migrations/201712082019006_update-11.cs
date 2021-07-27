namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update11 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Enquiries", new[] { "PublicUserId" });
            AlterColumn("dbo.Enquiries", "PublicUserId", c => c.Long(nullable: false));
            CreateIndex("dbo.Enquiries", "PublicUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Enquiries", new[] { "PublicUserId" });
            AlterColumn("dbo.Enquiries", "PublicUserId", c => c.Long());
            CreateIndex("dbo.Enquiries", "PublicUserId");
        }
    }
}
