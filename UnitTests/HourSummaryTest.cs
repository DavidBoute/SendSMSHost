using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Effort;
using System.Linq;
using SendSMSHost.Models;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class HourSummaryFactoryTest
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
                    TimeStamp = DateTime.Now.AddMinutes(-1)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 3,
                    TimeStamp = DateTime.Now.AddMinutes(-2)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 4,
                    TimeStamp = DateTime.Now.AddMinutes(-3)
                });
            db.Sms.Add(
                new Sms
                {
                    Id = Guid.NewGuid(),
                    ContactId = Guid.Parse(FREDDY_ID),
                    Message = "Test",
                    StatusId = 0,
                    TimeStamp = DateTime.Now.AddMinutes(-3)
                });
            db.Sms.Add(
            new Sms
            {
                Id = Guid.NewGuid(),
                ContactId = Guid.Parse(FREDDY_ID),
                Message = "Test",
                StatusId = 0,
                TimeStamp = DateTime.Now.AddMinutes(-3)
            });

            #endregion

            db.SaveChanges();

        }

        [TestMethod]
        public async Task HourSummaryCreateTest_IntervalCount()
        {
            // Arrange 
            ISummaryFactory summaryFactory = new HourSummaryFactory();

            // Act
            Summary summary = await summaryFactory.CreateAsync(db);

            // Assert
            int expectedValue = 4;
            int actualValue = summary.SummaryIntervalList.Count();
            Assert.IsTrue(actualValue == expectedValue,
                "Total number of intervals is wrong");
        }

        [TestMethod]
        public async Task HourSummaryCreateTest_IntervalInternalCount()
        {
            // Arrange 
            ISummaryFactory summaryFactory = new HourSummaryFactory();

            // Act
            Summary summary = await summaryFactory.CreateAsync(db);

            // Assert
            int expectedValue = 1;
            int actualValue = summary.SummaryIntervalList
                                .First(x => x.IntervalStart == DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                                                                                .AddMinutes(DateTime.Now.Minute))
                                .IntervalData
                                .Count();
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData, current minute");

            expectedValue = 1;
            actualValue = summary.SummaryIntervalList
                                .First(x => x.IntervalStart == DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                                                                                .AddMinutes(DateTime.Now.Minute-1))
                                .IntervalData
                                .Count();
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData, current minute -1");

            expectedValue = 1;
            actualValue = summary.SummaryIntervalList
                                .First(x => x.IntervalStart == DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                                                                                .AddMinutes(DateTime.Now.Minute-2))
                                .IntervalData
                                .Count();
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData, current minute -2");

            expectedValue = 2;
            actualValue = summary.SummaryIntervalList
                                .First(x => x.IntervalStart == DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                                                                                .AddMinutes(DateTime.Now.Minute-3))
                                .IntervalData
                                .Count();
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData, current minute -3");

            expectedValue = 1;
            actualValue = summary.SummaryIntervalList
                                .First(x => x.IntervalStart == DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                                                                                .AddMinutes(DateTime.Now.Minute-3))
                                .IntervalData
                                .First(y => y.StatusName == "Sent")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Sent, current minute -3");

            expectedValue = 2;
            actualValue = summary.SummaryIntervalList
                                .First(x => x.IntervalStart == DateTime.Now.Date.AddHours(DateTime.Now.Hour)
                                                                                .AddMinutes(DateTime.Now.Minute-3))
                                .IntervalData
                                .First(y => y.StatusName == "Error")
                                .StatusCount;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of IntervalData - Status: Error, current minute -3");
        }
    }
}
