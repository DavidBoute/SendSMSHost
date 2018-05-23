using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort;
using SendSMSHost.Models.Factory;
using SendSMSHost.Models;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class ForeverChartDataTest
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
                    Timestamp = DateTime.Now.AddDays(-1)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "3",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddDays(-2)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "4",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddDays(-3)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "1",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddDays(-3)
                });
            db.Log.Add(
                new Log
                {
                    SmsId = "2",
                    StatusName = "Created",
                    Timestamp = DateTime.Now.AddDays(-3)
                });

            #endregion

            db.SaveChanges();
        }

        [TestMethod]
        public void ForeverSummaryCreateChartDataTest_IntervalCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new ForeverChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 5;
            int actualValue = chartData.Labels.Length;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Label");
        }

        [TestMethod]
        public void ForeverSummaryCreateChartDataTest_StatusCreatedCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new ForeverChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 2;
            int actualValue = CountStatusInDataset(chartData, "Created");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Created");
        }

        [TestMethod]
        public void ForeverSummaryCreateChartDataTest_StatusQueuedCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new ForeverChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 1;
            int actualValue = CountStatusInDataset(chartData, "Queued");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Queued");
        }

        [TestMethod]
        public void ForeverSummaryCreateChartDataTest_StatusErrorCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new ForeverChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 1;
            int actualValue = CountStatusInDataset(chartData, "Error");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Error");
        }

        [TestMethod]
        public void ForeverSummaryCreateChartDataTest_StatusPendingCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new ForeverChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 0;
            int actualValue = CountStatusInDataset(chartData, "Pending");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Pending");
        }

        [TestMethod]
        public void ForeverSummaryCreateChartDataTest_StatusSentCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new ForeverChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 0;
            int actualValue = CountStatusInDataset(chartData, "Sent");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Sent");
        }

        public int CountStatusInDataset(ChartData chartData, string statusName)
        {
            int statusIndex = chartData.Labels.ToList().IndexOf(statusName);

            return chartData
                        .Datasets
                        .First()
                        .Data
                        .ElementAt(statusIndex);
        }
    }
}
