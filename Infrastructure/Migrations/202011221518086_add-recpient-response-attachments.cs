namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addrecpientresponseattachments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipientResponseAttachments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        FileName = c.String(),
                        OriginalFileName = c.String(),
                        RecipientId = c.Long(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Recipients", t => t.RecipientId)
                .Index(t => t.RecipientId)
                .Index(t => t.CreatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RecipientResponseAttachments", "RecipientId", "dbo.Recipients");
            DropForeignKey("dbo.RecipientResponseAttachments", "CreatorId", "dbo.ApplicationUsers");
            DropIndex("dbo.RecipientResponseAttachments", new[] { "CreatorId" });
            DropIndex("dbo.RecipientResponseAttachments", new[] { "RecipientId" });
            DropTable("dbo.RecipientResponseAttachments");
        }
    }
}
