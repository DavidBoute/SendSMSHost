﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort;
using SendSMSHost.Models.Factory;
using SendSMSHost.Models;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class WeekChartDataTest
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
        public void WeekSummaryCreateChartDataTest_IntervalCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 7;
            int actualValue = chartData.Labels.Length;
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Label");
        }

        [TestMethod]
        public void WeekSummaryCreateChartDataTest_StatusCreatedCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 2;
            int actualValue = CountStatusInDataset(chartData, "Created");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Created");
        }

        [TestMethod]
        public void WeekSummaryCreateChartDataTest_StatusQueuedCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 1;
            int actualValue = CountStatusInDataset(chartData, "Queued");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Queued");
        }

        [TestMethod]
        public void WeekSummaryCreateChartDataTest_StatusErrorCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 1;
            int actualValue = CountStatusInDataset(chartData, "Error");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Error");
        }

        [TestMethod]
        public void WeekSummaryCreateChartDataTest_StatusPendingCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

            // Act          
            ChartData chartData = summaryFactory.CreateChartData(db, includeDeleted: false);

            // Assert
            int expectedValue = 0;
            int actualValue = CountStatusInDataset(chartData, "Pending");
            Assert.IsTrue(actualValue == expectedValue,
                "Count of Status Pending");
        }

        [TestMethod]
        public void WeekSummaryCreateChartDataTest_StatusSentCount()
        {
            // Arrange  
            IChartDataFactory summaryFactory = new WeekChartDataFactory();

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
            return chartData
                        .Datasets
                        .FirstOrDefault(x => x.Label == statusName)
                        .Data
                        .Sum();
        }
    }
}
