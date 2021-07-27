namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update15 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Items", "CopiedFromId", "dbo.Items");
            DropIndex("dbo.Items", new[] { "CopiedFromId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Items", "CopiedFromId");
            AddForeignKey("dbo.Items", "CopiedFromId", "dbo.Items", "Id");
        }
    }
}
