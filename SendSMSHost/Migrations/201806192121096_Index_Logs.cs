namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Index_Logs : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Logs", "Operation", c => c.String(maxLength: 10));
            AlterColumn("dbo.Logs", "StatusName", c => c.String(maxLength: 10));
            CreateIndex("dbo.Logs", "Operation");
            CreateIndex("dbo.Logs", "StatusName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Logs", new[] { "StatusName" });
            DropIndex("dbo.Logs", new[] { "Operation" });
            AlterColumn("dbo.Logs", "StatusName", c => c.String());
            AlterColumn("dbo.Logs", "Operation", c => c.String());
        }
    }
}
