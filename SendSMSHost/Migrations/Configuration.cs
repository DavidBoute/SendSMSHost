namespace SendSMSHost.Migrations
{
    using SendSMSHost.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SendSMSHost.Models.SendSMSHostContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SendSMSHost.Models.SendSMSHostContext context)
        {
            context.Status.AddOrUpdate(x => x.Id,
                new Status { Id = 1, Name = "Created" },
                new Status { Id = 2, Name = "Queued" },
                new Status { Id = 3, Name = "Pending" },
                new Status { Id = 4, Name = "Sent" },
                new Status { Id = 0, Name = "Error" });

            context.Contacts.AddOrUpdate(x => x.Id,
                new Contact { Id = new Guid("6185B42F-7A64-4D9B-9098-B5E4503E75C7"), FirstName = "Freddy", LastName = "De Testaccount", Number = "+32494240152" });

            context.Sms.AddOrUpdate(x => x.Id,
                new Sms
                {
                    Id = new Guid("6B86A791-5D32-4281-BC4E-C8EEE8647817"),
                    ContactId = new Guid("6185B42F-7A64-4D9B-9098-B5E4503E75C7"),
                    Message = "Test",
                    StatusId = 1,
                    TimeStamp = DateTime.Now
                });

            // Naam van Key aanpassen, nodig voor verbinding MS Access (er mag geen dbo. in staan)
            context.Database.ExecuteSqlCommand(
                @"sp_rename '[dbo].[ImportSms].[PK_dbo.ImportSms]', 'PK_ImportSms'");
        }
    }
}
