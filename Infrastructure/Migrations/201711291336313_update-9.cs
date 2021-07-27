namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update9 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MembershipPlans", "MaxRequestsPerEnquiry");
            DropColumn("dbo.MembershipPlans", "MaxEnquiryFields");
            DropColumn("dbo.MembershipPlans", "MaxRequestFields");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MembershipPlans", "MaxRequestFields", c => c.Long(nullable: false));
            AddColumn("dbo.MembershipPlans", "MaxEnquiryFields", c => c.Long(nullable: false));
            AddColumn("dbo.MembershipPlans", "MaxRequestsPerEnquiry", c => c.Long(nullable: false));
        }
    }
}
