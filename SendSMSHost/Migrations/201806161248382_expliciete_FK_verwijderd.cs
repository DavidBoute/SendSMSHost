namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class expliciete_FK_verwijderd : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sms", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Sms", "StatusId", "dbo.Status");
            DropIndex("dbo.Sms", new[] { "StatusId" });
            DropIndex("dbo.Sms", new[] { "ContactId" });
            RenameColumn(table: "dbo.Sms", name: "ContactId", newName: "Contact_Id");
            RenameColumn(table: "dbo.Sms", name: "StatusId", newName: "Status_Id");
            AlterColumn("dbo.Sms", "Status_Id", c => c.Int());
            AlterColumn("dbo.Sms", "Contact_Id", c => c.Guid());
            CreateIndex("dbo.Sms", "Contact_Id");
            CreateIndex("dbo.Sms", "Status_Id");
            AddForeignKey("dbo.Sms", "Contact_Id", "dbo.Contacts", "Id");
            AddForeignKey("dbo.Sms", "Status_Id", "dbo.Status", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sms", "Status_Id", "dbo.Status");
            DropForeignKey("dbo.Sms", "Contact_Id", "dbo.Contacts");
            DropIndex("dbo.Sms", new[] { "Status_Id" });
            DropIndex("dbo.Sms", new[] { "Contact_Id" });
            AlterColumn("dbo.Sms", "Contact_Id", c => c.Guid(nullable: false));
            AlterColumn("dbo.Sms", "Status_Id", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Sms", name: "Status_Id", newName: "StatusId");
            RenameColumn(table: "dbo.Sms", name: "Contact_Id", newName: "ContactId");
            CreateIndex("dbo.Sms", "ContactId");
            CreateIndex("dbo.Sms", "StatusId");
            AddForeignKey("dbo.Sms", "StatusId", "dbo.Status", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Sms", "ContactId", "dbo.Contacts", "Id", cascadeDelete: true);
        }
    }
}
