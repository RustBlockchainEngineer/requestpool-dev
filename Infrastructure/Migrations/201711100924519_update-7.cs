namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update7 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Enquiries", new[] { "ProjectId" });
            AddColumn("dbo.Enquiries", "ParentId", c => c.Long());
            AddColumn("dbo.Enquiries", "IsTemplate", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Enquiries", "ProjectId", c => c.Long());
            CreateIndex("dbo.Enquiries", "ProjectId");
            CreateIndex("dbo.Enquiries", "ParentId");
            AddForeignKey("dbo.Enquiries", "ParentId", "dbo.Enquiries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enquiries", "ParentId", "dbo.Enquiries");
            DropIndex("dbo.Enquiries", new[] { "ParentId" });
            DropIndex("dbo.Enquiries", new[] { "ProjectId" });
            AlterColumn("dbo.Enquiries", "ProjectId", c => c.Long(nullable: false));
            DropColumn("dbo.Enquiries", "IsTemplate");
            DropColumn("dbo.Enquiries", "ParentId");
            CreateIndex("dbo.Enquiries", "ProjectId");
        }
    }
}
