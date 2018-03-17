using SendSMSHost.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class TestDataInitialiser
    {
        public void Seed(TestSendSMSHostContext db)
        {
            db.Status.AddOrUpdate(new Status { Id = 1, Name = "Created", DefaultColorHex = "#fdfdfe" });
            db.Status.AddOrUpdate(new Status { Id = 2, Name = "Queued", DefaultColorHex = "#bee5eb" });
            db.Status.AddOrUpdate(new Status { Id = 3, Name = "Pending", DefaultColorHex = "#ffeeba" });
            db.Status.AddOrUpdate(new Status { Id = 4, Name = "Sent", DefaultColorHex = "#c3e6cb" });
            db.Status.AddOrUpdate(new Status { Id = 0, Name = "Error", DefaultColorHex = "#f5c6cb" });

            Contact freddy = new Contact
            {
                Id = new Guid("6185B42F-7A64-4D9B-9098-B5E4503E75C7"),
                FirstName = "Freddy",
                LastName = "De Testaccount",
                Number = "+32494240152"
            };
            db.Contacts.AddOrUpdate(freddy);

            db.SaveChanges();
        }
    }
}
