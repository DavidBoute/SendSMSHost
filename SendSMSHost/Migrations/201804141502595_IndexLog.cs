namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IndexLog : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Logs", "SmsId", c => c.String(maxLength: 36));
            CreateIndex("dbo.Logs", "SmsId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Logs", new[] { "SmsId" });
            AlterColumn("dbo.Logs", "SmsId", c => c.String());
        }
    }
}
