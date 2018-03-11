using SendSMSHost.Models;
using SendSMSHost.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SendSMSHost.Controllers
{
    public class SummaryController : ApiController
    {
        private SendSMSHostContext db = new SendSMSHostContext();

        // GET: api/Summary
        // DurationId:
        // 0 = forever
        // 1 = week
        // 2 = day
        // 3 = hour
        // 4 = 15 minutes
        // 5 = 1 minute
        public SummaryIndexViewModel GetSummary(int durationId)
        {
            var VM = new SummaryIndexViewModel();

            SummaryParameters parameters;
            switch (durationId)
            {
                case 0: //forever - nog even on hold, meer complex dan verwacht
                    parameters = new SummaryParametersForeverVariable(24);
                    break;
                case 1: //week
                    parameters = new SummaryParametersFixed(7, TimeSpan.FromDays(1));
                    break;
                case 2: //day
                    parameters = new SummaryParametersFixed(24, TimeSpan.FromHours(1));
                    break;
                case 3: //hour
                    parameters = new SummaryParametersFixed(12, TimeSpan.FromMinutes(5));
                    break;
                case 4: //15 minutes
                    parameters = new SummaryParametersFixed(15, TimeSpan.FromMinutes(1));
                    break;
                case 5: //1 minute
                    parameters = new SummaryParametersFixed(12, TimeSpan.FromSeconds(5));
                    break;
                default:
                    throw new ArgumentException("The provided durationId is not valid.");
            }

            return VM;
        }

        private abstract class SummaryParameters
        {
            internal DateTime SummaryStartTime { get; set; }
            internal int IntervalCount { get; set; }
            internal TimeSpan IntervalDuration { get; set; }
        }

        private class SummaryParametersFixed : SummaryParameters
        {
            public SummaryParametersFixed(int intervalCount, TimeSpan intervalDuration)
            {
                IntervalCount = intervalCount;
                IntervalDuration = IntervalDuration;

                double summaryDuration = intervalCount * IntervalDuration.TotalSeconds;

                SummaryStartTime = DateTime.Today.AddSeconds(-summaryDuration);
            }
        }

        /// <summary>
        /// Parameters voor 'Forever' durationId, werkt per maand, variabel aantal dagen per maand
        /// </summary>
        private class SummaryParametersForeverVariable : SummaryParameters
        {
            public SummaryParametersForeverVariable(int intervalCount)
            {
                IntervalCount = intervalCount;
                IntervalDuration = IntervalDuration;

                double summaryDuration = intervalCount * IntervalDuration.TotalSeconds;

                SummaryStartTime = DateTime.Today.AddSeconds(-summaryDuration);
            }
        }


    }
}
