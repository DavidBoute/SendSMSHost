namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImportSms_met_namen : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ImportSms", "ContactFirstName", c => c.String());
            AddColumn("dbo.ImportSms", "ContactLastName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ImportSms", "ContactLastName");
            DropColumn("dbo.ImportSms", "ContactFirstName");
        }
    }
}
