using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort;
using SendSMSHost.Models.Factory;
using SendSMSHost.Models;

namespace UnitTests
{
    [TestClass]
    public class WeekChartDataTest
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
        public void WeekSummaryCreateChartDataTest_IntervalCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 7;
            int actualValue = chartData.Labels.Length;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Label");
        }
    }
}
