namespace Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        Otp = c.String(),
                        Photo = c.String(),
                        Remarks = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CreatorId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.ApplicationUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ApplicationUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ApplicationUserRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.ApplicationUsers", t => t.UserId)
                .ForeignKey("dbo.ApplicationRoles", t => t.RoleId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                        Fax = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        Website = c.String(),
                        Profile = c.String(),
                        Remarks = c.String(),
                        PublicUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Code = c.String(),
                        Description = c.String(),
                        Remarks = c.String(),
                        ClientId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.ClientId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Enquiries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                        RevisionNumber = c.String(),
                        Subject = c.String(),
                        Description = c.String(),
                        Remarks = c.String(),
                        EnquiryTypeId = c.Int(nullable: false),
                        ProjectId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.EnquiryTypes", t => t.EnquiryTypeId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.EnquiryTypeId)
                .Index(t => t.ProjectId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.EnquiryAttachments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        FileName = c.String(),
                        OriginalFileName = c.String(),
                        EnquiryId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Enquiries", t => t.EnquiryId)
                .Index(t => t.EnquiryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.EnquiryTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PublicUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                        RevisionNumber = c.String(),
                        Subject = c.String(),
                        Description = c.String(),
                        Remarks = c.String(),
                        ItemTypeId = c.Int(),
                        EnquiryId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Enquiries", t => t.EnquiryId)
                .ForeignKey("dbo.ItemTypes", t => t.ItemTypeId)
                .Index(t => t.ItemTypeId)
                .Index(t => t.EnquiryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.ItemTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PublicUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Memberships",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Remarks = c.String(),
                        MembershipPlanId = c.Int(nullable: false),
                        DowngradeToMembershipPlanId = c.Int(),
                        PublicUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.MembershipPlans", t => t.DowngradeToMembershipPlanId)
                .ForeignKey("dbo.MembershipPlans", t => t.MembershipPlanId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.MembershipPlanId)
                .Index(t => t.DowngradeToMembershipPlanId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.MembershipPlans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        MaxEnquiriesPerMonth = c.Int(nullable: false),
                        MaxRequestsPerEnquiry = c.Int(nullable: false),
                        MaxItemsPerEnquiry = c.Int(nullable: false),
                        MaxInvitationsPerMonth = c.Int(nullable: false),
                        MaxContactsPerInvitation = c.Int(nullable: false),
                        MaxEnquiryFields = c.Int(nullable: false),
                        MaxRequestFields = c.Int(nullable: false),
                        MaxItemFields = c.Int(nullable: false),
                        CostPerMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CostPerYear = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remarks = c.String(),
                        IsPublic = c.Boolean(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Rank = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CountryId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CountryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Rank = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Regions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Rank = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CityId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cities", t => t.CityId)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CityId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Phone = c.String(),
                        Fax = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        Profile = c.String(),
                        Remarks = c.String(),
                        PublicUserId = c.Int(nullable: false),
                        ContactTypeId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ContactTypes", t => t.ContactTypeId)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PublicUserId)
                .Index(t => t.ContactTypeId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.ContactTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        PublicUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.EnquiryResponses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RecipientId = c.Int(nullable: false),
                        EnquiryId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Enquiries", t => t.EnquiryId)
                .ForeignKey("dbo.ApplicationUsers", t => t.RecipientId)
                .Index(t => t.RecipientId)
                .Index(t => t.EnquiryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Description = c.String(),
                        PostDate = c.DateTime(),
                        IsDraft = c.Boolean(nullable: false),
                        HasErrors = c.Boolean(nullable: false),
                        EnquiryId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Enquiries", t => t.EnquiryId)
                .Index(t => t.EnquiryId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Recipients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsFailed = c.Boolean(nullable: false),
                        FailureReason = c.String(),
                        ContactId = c.Int(nullable: false),
                        PublicUserId = c.Int(),
                        InvitationId = c.Int(nullable: false),
                        IsDraftResponse = c.Boolean(nullable: false),
                        ResponseSubmitDate = c.DateTime(),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Invitations", t => t.InvitationId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.ContactId)
                .Index(t => t.PublicUserId)
                .Index(t => t.InvitationId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.ItemResponses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Response = c.String(),
                        RecipientId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.Items", t => t.ItemId)
                .ForeignKey("dbo.Recipients", t => t.RecipientId)
                .Index(t => t.RecipientId)
                .Index(t => t.ItemId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Otps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Phone = c.String(),
                        Username = c.String(),
                        Purpose = c.String(),
                        IP = c.String(),
                        Code = c.String(),
                        IsUsed = c.Boolean(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                        IsSetByAdmin = c.Boolean(nullable: false),
                        UseDate = c.DateTime(),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.ApplicationRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DefaultMembershipPeriod = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .Index(t => t.CreatorId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Color = c.String(),
                        Rank = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        PublicUserId = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        LastUpdateDate = c.DateTime(nullable: false),
                        CreatorId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CreatorId)
                .ForeignKey("dbo.ApplicationUsers", t => t.PublicUserId)
                .Index(t => t.PublicUserId)
                .Index(t => t.CreatorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Status", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Status", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Settings", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserRoles", "RoleId", "dbo.ApplicationRoles");
            DropForeignKey("dbo.Otps", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Recipients", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemResponses", "RecipientId", "dbo.Recipients");
            DropForeignKey("dbo.ItemResponses", "ItemId", "dbo.Items");
            DropForeignKey("dbo.ItemResponses", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Recipients", "InvitationId", "dbo.Invitations");
            DropForeignKey("dbo.Recipients", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Recipients", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Invitations", "EnquiryId", "dbo.Enquiries");
            DropForeignKey("dbo.Invitations", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.EnquiryResponses", "RecipientId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.EnquiryResponses", "EnquiryId", "dbo.Enquiries");
            DropForeignKey("dbo.EnquiryResponses", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Contacts", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Contacts", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Contacts", "ContactTypeId", "dbo.ContactTypes");
            DropForeignKey("dbo.ContactTypes", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ContactTypes", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Regions", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Regions", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Cities", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Countries", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Cities", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Memberships", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Memberships", "MembershipPlanId", "dbo.MembershipPlans");
            DropForeignKey("dbo.Memberships", "DowngradeToMembershipPlanId", "dbo.MembershipPlans");
            DropForeignKey("dbo.MembershipPlans", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Memberships", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Clients", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Enquiries", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Items", "ItemTypeId", "dbo.ItemTypes");
            DropForeignKey("dbo.ItemTypes", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ItemTypes", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Items", "EnquiryId", "dbo.Enquiries");
            DropForeignKey("dbo.Items", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Enquiries", "EnquiryTypeId", "dbo.EnquiryTypes");
            DropForeignKey("dbo.EnquiryTypes", "PublicUserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.EnquiryTypes", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Enquiries", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.EnquiryAttachments", "EnquiryId", "dbo.Enquiries");
            DropForeignKey("dbo.EnquiryAttachments", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Projects", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.Projects", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Clients", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserRoles", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserLogins", "UserId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUsers", "CreatorId", "dbo.ApplicationUsers");
            DropForeignKey("dbo.ApplicationUserClaims", "UserId", "dbo.ApplicationUsers");
            DropIndex("dbo.Status", new[] { "CreatorId" });
            DropIndex("dbo.Status", new[] { "PublicUserId" });
            DropIndex("dbo.Settings", new[] { "CreatorId" });
            DropIndex("dbo.ApplicationRoles", "RoleNameIndex");
            DropIndex("dbo.Otps", new[] { "CreatorId" });
            DropIndex("dbo.ItemResponses", new[] { "CreatorId" });
            DropIndex("dbo.ItemResponses", new[] { "ItemId" });
            DropIndex("dbo.ItemResponses", new[] { "RecipientId" });
            DropIndex("dbo.Recipients", new[] { "CreatorId" });
            DropIndex("dbo.Recipients", new[] { "InvitationId" });
            DropIndex("dbo.Recipients", new[] { "PublicUserId" });
            DropIndex("dbo.Recipients", new[] { "ContactId" });
            DropIndex("dbo.Invitations", new[] { "CreatorId" });
            DropIndex("dbo.Invitations", new[] { "EnquiryId" });
            DropIndex("dbo.EnquiryResponses", new[] { "CreatorId" });
            DropIndex("dbo.EnquiryResponses", new[] { "EnquiryId" });
            DropIndex("dbo.EnquiryResponses", new[] { "RecipientId" });
            DropIndex("dbo.ContactTypes", new[] { "CreatorId" });
            DropIndex("dbo.ContactTypes", new[] { "PublicUserId" });
            DropIndex("dbo.Contacts", new[] { "CreatorId" });
            DropIndex("dbo.Contacts", new[] { "ContactTypeId" });
            DropIndex("dbo.Contacts", new[] { "PublicUserId" });
            DropIndex("dbo.Regions", new[] { "CreatorId" });
            DropIndex("dbo.Regions", new[] { "CityId" });
            DropIndex("dbo.Countries", new[] { "CreatorId" });
            DropIndex("dbo.Cities", new[] { "CreatorId" });
            DropIndex("dbo.Cities", new[] { "CountryId" });
            DropIndex("dbo.MembershipPlans", new[] { "CreatorId" });
            DropIndex("dbo.Memberships", new[] { "CreatorId" });
            DropIndex("dbo.Memberships", new[] { "PublicUserId" });
            DropIndex("dbo.Memberships", new[] { "DowngradeToMembershipPlanId" });
            DropIndex("dbo.Memberships", new[] { "MembershipPlanId" });
            DropIndex("dbo.ItemTypes", new[] { "CreatorId" });
            DropIndex("dbo.ItemTypes", new[] { "PublicUserId" });
            DropIndex("dbo.Items", new[] { "CreatorId" });
            DropIndex("dbo.Items", new[] { "EnquiryId" });
            DropIndex("dbo.Items", new[] { "ItemTypeId" });
            DropIndex("dbo.EnquiryTypes", new[] { "CreatorId" });
            DropIndex("dbo.EnquiryTypes", new[] { "PublicUserId" });
            DropIndex("dbo.EnquiryAttachments", new[] { "CreatorId" });
            DropIndex("dbo.EnquiryAttachments", new[] { "EnquiryId" });
            DropIndex("dbo.Enquiries", new[] { "CreatorId" });
            DropIndex("dbo.Enquiries", new[] { "ProjectId" });
            DropIndex("dbo.Enquiries", new[] { "EnquiryTypeId" });
            DropIndex("dbo.Projects", new[] { "CreatorId" });
            DropIndex("dbo.Projects", new[] { "ClientId" });
            DropIndex("dbo.Clients", new[] { "CreatorId" });
            DropIndex("dbo.Clients", new[] { "PublicUserId" });
            DropIndex("dbo.ApplicationUserRoles", new[] { "RoleId" });
            DropIndex("dbo.ApplicationUserRoles", new[] { "UserId" });
            DropIndex("dbo.ApplicationUserLogins", new[] { "UserId" });
            DropIndex("dbo.ApplicationUserClaims", new[] { "UserId" });
            DropIndex("dbo.ApplicationUsers", "UserNameIndex");
            DropIndex("dbo.ApplicationUsers", new[] { "CreatorId" });
            DropTable("dbo.Status");
            DropTable("dbo.Settings");
            DropTable("dbo.ApplicationRoles");
            DropTable("dbo.Otps");
            DropTable("dbo.ItemResponses");
            DropTable("dbo.Recipients");
            DropTable("dbo.Invitations");
            DropTable("dbo.EnquiryResponses");
            DropTable("dbo.ContactTypes");
            DropTable("dbo.Contacts");
            DropTable("dbo.Regions");
            DropTable("dbo.Countries");
            DropTable("dbo.Cities");
            DropTable("dbo.MembershipPlans");
            DropTable("dbo.Memberships");
            DropTable("dbo.ItemTypes");
            DropTable("dbo.Items");
            DropTable("dbo.EnquiryTypes");
            DropTable("dbo.EnquiryAttachments");
            DropTable("dbo.Enquiries");
            DropTable("dbo.Projects");
            DropTable("dbo.Clients");
            DropTable("dbo.ApplicationUserRoles");
            DropTable("dbo.ApplicationUserLogins");
            DropTable("dbo.ApplicationUserClaims");
            DropTable("dbo.ApplicationUsers");
        }
    }
}
