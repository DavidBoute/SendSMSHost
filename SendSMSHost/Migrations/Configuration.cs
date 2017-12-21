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
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Contacts.AddOrUpdate(x => x.Id,
                new Contact { Id = new Guid("6185B42F-7A64-4D9B-9098-B5E4503E75C7"), FirstName = "Freddy", LastName = "De Testaccount", Number = "123" });

            context.Status.AddOrUpdate(x => x.Id,
                new Status { Id = 1, Name = "Created" },
                new Status { Id = 2, Name = "Queued" },
                new Status { Id = 3, Name = "Pending" },
                new Status { Id = 4, Name = "Sent" },
                new Status { Id = 0, Name = "Error" });

            context.Sms.AddOrUpdate(x => x.Id,
                new Sms
                {
                    Id = new Guid("6B86A791-5D32-4281-BC4E-C8EEE8647817"),
                    ContactId = new Guid("6185B42F-7A64-4D9B-9098-B5E4503E75C7"),
                    Message = "Test",
                    StatusId = 1,
                    TimeStamp = DateTime.Now
                });

        }
    }
}
