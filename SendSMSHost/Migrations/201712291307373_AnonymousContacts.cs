namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnonymousContacts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "IsAnonymous", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contacts", "IsAnonymous");
        }
    }
}
