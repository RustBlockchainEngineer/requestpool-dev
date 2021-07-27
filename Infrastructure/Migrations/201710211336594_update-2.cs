namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ItemDynamicProperties",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ItemId = c.Long(nullable: false),
                        DynamicPropertyId = c.Long(nullable: false),
                        Value = c.String(),
                        IsApplicable = c.Boolean(nullable: false),
                        IsReadOnly = c.Boolean(nullable: false),
                        IsRequired = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ItemsDynamicProperties", t => t.DynamicPropertyId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .Index(t => t.ItemId)
                .Index(t => t.DynamicPropertyId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.ItemsDynamicProperties",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        PropertyTypeId = c.Long(nullable: false),
                        PublicUserId = c.Long(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.PropertyTypes", t => t.PropertyTypeId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PropertyTypeId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.PropertyTypes",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        UiRegex = c.String(),
                        DbRegex = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.ItemDynamicPropertyResponses",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ItemResponseId = c.Long(nullable: false),
                        PropertyId = c.Long(nullable: false),
                        Response = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ItemResponses", t => t.ItemResponseId)
                .ForeignKey("dbo.ItemsDynamicProperties", t => t.PropertyId)
                .Index(t => t.ItemResponseId)
                .Index(t => t.PropertyId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.EnquiryItemsDynamicProperties",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        EnquiryId = c.Long(nullable: false),
                        PropertyId = c.Long(nullable: false),
                        Rank = c.Int(nullable: false),
                        IsVisible = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Long(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Enquiries", t => t.EnquiryId)
                .ForeignKey("dbo.ItemsDynamicProperties", t => t.PropertyId)
                .Index(t => t.EnquiryId)
                .Index(t => t.PropertyId)
                .Index(t => t.CreatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EnquiryItemsDynamicProperties", "PropertyId", "dbo.ItemsDynamicProperties");
            DropForeignKey("dbo.EnquiryItemsDynamicProperties", "EnquiryId", "dbo.Enquiries");
            DropForeignKey("dbo.EnquiryItemsDynamicProperties", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemDynamicPropertyResponses", "PropertyId", "dbo.ItemsDynamicProperties");
            DropForeignKey("dbo.ItemDynamicPropertyResponses", "ItemResponseId", "dbo.ItemResponses");
            DropForeignKey("dbo.ItemDynamicPropertyResponses", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemDynamicProperties", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemDynamicProperties", "DynamicPropertyId", "dbo.ItemsDynamicProperties");
            DropForeignKey("dbo.ItemsDynamicProperties", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemsDynamicProperties", "PropertyTypeId", "dbo.PropertyTypes");
            DropForeignKey("dbo.PropertyTypes", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemsDynamicProperties", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemDynamicProperties", "CreatorId", "dbo.ApplicationUsers");
            DropIndex("dbo.EnquiryItemsDynamicProperties", new[] { "CreatorId" });
            DropIndex("dbo.EnquiryItemsDynamicProperties", new[] { "PropertyId" });
            DropIndex("dbo.EnquiryItemsDynamicProperties", new[] { "EnquiryId" });
            DropIndex("dbo.ItemDynamicPropertyResponses", new[] { "CreatorId" });
            DropIndex("dbo.ItemDynamicPropertyResponses", new[] { "PropertyId" });
            DropIndex("dbo.ItemDynamicPropertyResponses", new[] { "ItemResponseId" });
            DropIndex("dbo.PropertyTypes", new[] { "CreatorId" });
            DropIndex("dbo.ItemsDynamicProperties", new[] { "CreatorId" });
            DropIndex("dbo.ItemsDynamicProperties", new[] { "PublicUserId" });
            DropIndex("dbo.ItemsDynamicProperties", new[] { "PropertyTypeId" });
            DropIndex("dbo.ItemDynamicProperties", new[] { "CreatorId" });
            DropIndex("dbo.ItemDynamicProperties", new[] { "DynamicPropertyId" });
            DropIndex("dbo.ItemDynamicProperties", new[] { "ItemId" });
            DropTable("dbo.EnquiryItemsDynamicProperties");
            DropTable("dbo.ItemDynamicPropertyResponses");
            DropTable("dbo.PropertyTypes");
            DropTable("dbo.ItemsDynamicProperties");
            DropTable("dbo.ItemDynamicProperties");
        }
    }
}
