using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SendSMSHost.Models;
using System.Threading.Tasks;
using Effort;
using System.Linq;

namespace SummaryFactoryTest
{
    [TestClass]
    public class WeekSummaryFactoryTest
    {
        private TestSendSMSHostContext db;

        [TestInitialize]
        public void Initialize()
        {
            var connection = DbConnectionFactory.CreateTransient();
            db = new TestSendSMSHostContext(connection);

            db.Status.Add(new Status { Id = 1, Name = "Created" });
            db.Status.Add(new Status { Id = 2, Name = "Queued" });
            db.Status.Add(new Status { Id = 3, Name = "Pending" });
            db.Status.Add(new Status { Id = 4, Name = "Sent" });
            db.Status.Add(new Status { Id = 0, Name = "Error" });
        }

        [TestMethod]
        public async Task CreateTest()
        {
            // Arrange 
            Contact freddy = new Contact
            {
                Id = new Guid("6185B42F-7A64-4D9B-9098-B5E4503E75C7"),
                FirstName = "Freddy",
                LastName = "De Testaccount",
                Number = "+32494240152"
            };
            db.Contacts.Add(freddy);
                

            db.Sms.Add(
                new Sms
                {
                    Id = new Guid("25838A4B-1B32-4153-ADF0-F1F4501EDE38"),
                    ContactId = freddy.Id,
                    Message = "Test",
                    StatusId = 1,
                    TimeStamp = DateTime.Now
                });
            db.Sms.Add(
                new Sms
                {
                    Id = new Guid("84DB1788-6784-4FFC-9BFA-60849B273959"),
                    ContactId = freddy.Id,
                    Message = "Test",
                    StatusId = 2,
                    TimeStamp = DateTime.Now.AddDays(-1)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = new Guid("FBFC3FD1-77C9-49FB-BBCF-C667B67FC3AD"),
                    ContactId = freddy.Id,
                    Message = "Test",
                    StatusId = 3,
                    TimeStamp = DateTime.Now.AddDays(-2)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = new Guid("1561F585-6E5E-492F-8A24-4D447FDF4CBC"),
                    ContactId = freddy.Id,
                    Message = "Test",
                    StatusId = 4,
                    TimeStamp = DateTime.Now.AddDays(-3)
                });


            // Act
            ISummaryFactory summaryFactory = new WeekSummaryFactory();
            ISummary summary = await summaryFactory.CreateAsync(db);

            // Assert
            int expectedValue = 5;
            int actualValue = db.Sms.Count();
            Assert.IsTrue(actualValue == expectedValue);
        }
    }
}
