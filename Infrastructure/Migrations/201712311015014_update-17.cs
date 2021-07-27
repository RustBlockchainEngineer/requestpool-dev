namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update17 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Status", "PublicUserId", "dbo.ApplicationUsers");
            DropIndex("dbo.Status", new[] { "PublicUserId" });
            AddColumn("dbo.Enquiries", "StatusId", c => c.Long());
            CreateIndex("dbo.Enquiries", "StatusId");
            AddForeignKey("dbo.Enquiries", "StatusId", "dbo.Status", "Id");
            DropColumn("dbo.Status", "PublicUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Status", "PublicUserId", c => c.Long(nullable: false));
            DropForeignKey("dbo.Enquiries", "StatusId", "dbo.Status");
            DropIndex("dbo.Enquiries", new[] { "StatusId" });
            DropColumn("dbo.Enquiries", "StatusId");
            CreateIndex("dbo.Status", "PublicUserId");
            AddForeignKey("dbo.Status", "PublicUserId", "dbo.ApplicationUsers", "Id");
        }
    }
}
