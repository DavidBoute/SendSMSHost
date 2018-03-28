namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Number = c.String(),
                        IsAnonymous = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sms",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Message = c.String(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        StatusId = c.Int(nullable: false),
                        ContactId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: true)
                .Index(t => t.StatusId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        DefaultColorHex = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImportSms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        ContactFirstName = c.String(),
                        ContactLastName = c.String(),
                        ContactNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        SmsId = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        Operation = c.String(),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sms", "StatusId", "dbo.Status");
            DropForeignKey("dbo.Sms", "ContactId", "dbo.Contacts");
            DropIndex("dbo.Sms", new[] { "ContactId" });
            DropIndex("dbo.Sms", new[] { "StatusId" });
            DropTable("dbo.Logs");
            DropTable("dbo.ImportSms");
            DropTable("dbo.Status");
            DropTable("dbo.Sms");
            DropTable("dbo.Contacts");
        }
    }
}
