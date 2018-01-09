namespace SendSMSHost.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class import : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ImportSms");
        }
    }
}
