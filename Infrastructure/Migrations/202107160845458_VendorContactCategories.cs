namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VendorContactCategories : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VendorContactCategories",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ContactId = c.Long(nullable: false),
                        ContactTypeId = c.Long(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .ForeignKey("dbo.ContactTypes", t => t.ContactTypeId)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.ContactId)
                .Index(t => t.ContactTypeId)
                .Index(t => t.CreatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VendorContactCategories", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.VendorContactCategories", "ContactTypeId", "dbo.ContactTypes");
            DropForeignKey("dbo.VendorContactCategories", "ContactId", "dbo.Contacts");
            DropIndex("dbo.VendorContactCategories", new[] { "CreatorId" });
            DropIndex("dbo.VendorContactCategories", new[] { "ContactTypeId" });
            DropIndex("dbo.VendorContactCategories", new[] { "ContactId" });
            DropTable("dbo.VendorContactCategories");
        }
    }
}
