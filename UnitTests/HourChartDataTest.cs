using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort;
using SendSMSHost.Models.Factory;
using SendSMSHost.Models;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class HourChartDataTest
    {
        private TestSendSMSHostContext db;

        [TestInitialize]
        public void Initialize()
        {
            var connection = DbConnectionFactory.CreateTransient();
            db = new TestSendSMSHostContext(connection);

            var init = new TestDataInitialiser();
            init.Seed(db);

            #region Add Logs

            db.Log.Add(
                new Log
                {
                    SmsId = "1",
                    StatusName = "Queued",
                    Timestamp = DateTime.Now
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "2",
                    StatusName = "Error",
                    Timestamp = DateTime.Now.AddMinutes(-15)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "3",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddMinutes(-25)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "4",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddMinutes(-30)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "1",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddMinutes(-30)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "2",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddMinutes(-30)
                });

            #endregion

            db.SaveChanges();
        }

        [TestMethod]
        public void HourSummaryCreateChartDataTest_IntervalCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new HourChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 12;
            int actualValue = chartData.Labels.Length;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Label");
        }

        [TestMethod]
        public void HourSummaryCreateChartDataTest_StatusCreatedCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new HourChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 2;
            int actualValue = CountStatusInDataset(chartData, "Created");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Created");
        }

        [TestMethod]
        public void HourSummaryCreateChartDataTest_StatusQueuedCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new HourChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 1;
            int actualValue = CountStatusInDataset(chartData, "Queued");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Queued");
        }

        [TestMethod]
        public void HourSummaryCreateChartDataTest_StatusErrorCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new HourChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 1;
            int actualValue = CountStatusInDataset(chartData, "Error");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Error");
        }

        [TestMethod]
        public void HourSummaryCreateChartDataTest_StatusPendingCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new HourChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 0;
            int actualValue = CountStatusInDataset(chartData, "Pending");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Pending");
        }

        [TestMethod]
        public void HourSummaryCreateChartDataTest_StatusSentCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new HourChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db);

            // Assert
            int expectedValue = 0;
            int actualValue = CountStatusInDataset(chartData, "Sent");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Sent");
        }

        public int CountStatusInDataset(ChartData chartData, string statusName)
        {
            return chartData
                        .Datasets
                        .FirstOrDefault(x => x.Label == statusName)
                        .Data
                        .Sum();
        }
    }
}
