using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort;
using SendSMSHost.Models;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ForeverSummaryTest
    {
        private TestSendSMSHostContext db;

        const string FREDDY_ID = "6185B42F-7A64-4D9B-9098-B5E4503E75C7";

        [TestInitialize]
        public void Initialize()
        {
            var connection = DbConnectionFactory.CreateTransient();
            db = new TestSendSMSHostContext(connection);

            var init = new TestDataInitialiser();
            init.Seed(db);

            #region Add Sms

            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 1,
                    TimeStamp = DateTime.Now
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 2,
                    TimeStamp = DateTime.Now.AddDays(-1)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 3,
                    TimeStamp = DateTime.Now.AddDays(-2)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 4,
                    TimeStamp = DateTime.Now.AddDays(-3)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 0,
                    TimeStamp = DateTime.Now.AddDays(-3)
                });
            db.Sms.Add(
            new Sms
            {
                Id = Guid.NewGuid(),
                ContactId = Guid.Parse(FREDDY_ID),
                Message = "Test",
                StatusId = 0,
                TimeStamp = DateTime.Now.AddDays(-3)
            });

            #endregion

            db.SaveChanges();
        }

        [TestMethod]
        public async Task ForeverSummaryCreateTest_IntervalCount()
        {
            // Arrange  
            ISummaryFactory summaryFactory = new ForeverSummaryFactory();

            // Act          
            Summary summary = await summaryFactory.CreateAsync(db);

            // Assert
            int expectedValue = 1;
            int actualValue = summary.SummaryIntervalList.Count();
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalList");
        }

        [TestMethod]
        public async Task ForeverSummaryCreateTest_IntervalInternalCount()
        {
            // Arrange 
            ISummaryFactory summaryFactory = new ForeverSummaryFactory();

            // Act
            Summary summary = await summaryFactory.CreateAsync(db);

            // Assert
            int expectedValue = 1;
            int actualValue = summary.SummaryIntervalList
                                .First()
                                .IntervalData
                                .First(y => y.StatusName == "Created")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Created");

            expectedValue = 1;
            actualValue = summary.SummaryIntervalList
                                .First()
                                .IntervalData
                                .First(y => y.StatusName == "Pending")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Pending");

            expectedValue = 1;
            actualValue = summary.SummaryIntervalList
                                .First()
                                .IntervalData
                                .First(y => y.StatusName == "Queued")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Queued");

            expectedValue = 1;
            actualValue = summary.SummaryIntervalList
                                .First()
                                .IntervalData
                                .First(y => y.StatusName == "Sent")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Sent");

            expectedValue = 2;
            actualValue = summary.SummaryIntervalList
                                .First()
                                .IntervalData
                                .First(y => y.StatusName == "Error")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Error");
        }
    }
}