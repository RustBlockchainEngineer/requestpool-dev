namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Enquiries", "ClientId", c => c.Long());
            AddColumn("dbo.Enquiries", "PublicUserId", c => c.Long());
            AddColumn("dbo.Items", "CopiedFromId", c => c.Long());
            CreateIndex("dbo.Enquiries", "ClientId");
            CreateIndex("dbo.Enquiries", "PublicUserId");
            CreateIndex("dbo.Items", "CopiedFromId");
            AddForeignKey("dbo.Enquiries", "ClientId", "dbo.Clients", "Id");
            AddForeignKey("dbo.Items", "CopiedFromId", "dbo.Items", "Id");
            AddForeignKey("dbo.Enquiries", "PublicUserId", "dbo.ApplicationUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enquiries", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Items", "CopiedFromId", "dbo.Items");
            DropForeignKey("dbo.Enquiries", "ClientId", "dbo.Clients");
            DropIndex("dbo.Items", new[] { "CopiedFromId" });
            DropIndex("dbo.Enquiries", new[] { "PublicUserId" });
            DropIndex("dbo.Enquiries", new[] { "ClientId" });
            DropColumn("dbo.Items", "CopiedFromId");
            DropColumn("dbo.Enquiries", "PublicUserId");
            DropColumn("dbo.Enquiries", "ClientId");
        }
    }
}
