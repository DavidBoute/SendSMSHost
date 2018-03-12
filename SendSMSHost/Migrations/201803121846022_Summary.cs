namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Summary : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.Sms", "StatusId", "dbo.Status");
            //DropPrimaryKey("dbo.Status");
            AlterColumn("dbo.Status", "Id", c => c.Int(nullable: false));
            //AddPrimaryKey("dbo.Status", "Id");
            //AddForeignKey("dbo.Sms", "StatusId", "dbo.Status", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.Sms", "StatusId", "dbo.Status");
            //DropPrimaryKey("dbo.Status");
            AlterColumn("dbo.Status", "Id", c => c.Int(nullable: false, identity: true));
            //AddPrimaryKey("dbo.Status", "Id");
            //AddForeignKey("dbo.Sms", "StatusId", "dbo.Status", "Id", cascadeDelete: true);
        }
    }
}
