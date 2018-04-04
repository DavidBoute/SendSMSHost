namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class import_without_contact : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ImportSms", "ContactFirstName");
            DropColumn("dbo.ImportSms", "ContactLastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ImportSms", "ContactLastName", c => c.String());
            AddColumn("dbo.ImportSms", "ContactFirstName", c => c.String());
        }
    }
}
