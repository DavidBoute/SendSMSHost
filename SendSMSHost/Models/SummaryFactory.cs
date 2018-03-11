using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SendSMSHost.Models
{
    public interface ISummaryFactory
    {
        Task<Summary> CreateAsync(ISendSMSHostContext db);
    }

    public class Summary
    {
        public List<SummaryInterval> SummaryIntervalList { get; set; }
    }

    public class SummaryInterval
    {
        public List<SummaryStatusCount> IntervalData { get; set; }
        public String IntervalName { get; set; }
        public DateTime IntervalStart { get; set; }
    }

    public class SummaryStatusCount
    {
        public string StatusName { get; set; }
        public int StatusCount { get; set; }
    }

    public class ForeverSummaryFactory : ISummaryFactory
    {
        public async Task<Summary> CreateAsync(ISendSMSHostContext db)
        {
            Summary summary = new Summary();

            var smsList = db.Sms
                .Include("Status")
                .GroupBy(x => x.Status.Name)
                .Select(grp => new SummaryStatusCount
                {
                    StatusName = grp.Key,
                    StatusCount = grp.Count()
                })
                .ToList();

            SummaryInterval interval = new SummaryInterval()
            {
                IntervalName = "Forever",
                IntervalData = smsList,
                IntervalStart = db.Sms.Min(x => x.TimeStamp).Date
            };

            List<SummaryInterval> intervalList = new List<SummaryInterval>();
            intervalList.Add(interval);

            summary.SummaryIntervalList = intervalList;

            return summary;
        }
    }

    public class WeekSummaryFactory : ISummaryFactory
    {
        public async Task<Summary> CreateAsync(ISendSMSHostContext db)
        {
            Summary summary = new Summary();

            DateTime startTime = DateTime.Today.AddDays(-7);

            var smsList = db.Sms
                .Include("Status")
                .Where(x => x.TimeStamp > startTime)
                .GroupBy(x => DbFunctions.TruncateTime(x.TimeStamp))
                .AsEnumerable()
                .Select(grp => new SummaryInterval()
                {
                    IntervalName = grp.Key.Value.DayOfWeek.ToString(),
                    IntervalData = grp.GroupBy(x => x.Status.Name)
                                        .Select(x => new SummaryStatusCount
                                        {
                                            StatusName = x.Key,
                                            StatusCount = x.Count()
                                        })
                                    .ToList(),
                    IntervalStart = grp.Key.Value
                })
                .ToList();

            summary.SummaryIntervalList = smsList;
            return summary;
        }
    }

    public class DaySummaryFactory : ISummaryFactory
    {
        public async Task<Summary> CreateAsync(ISendSMSHostContext db)
        {
            Summary summary = new Summary();

            DateTime startTime = DateTime.Today;

            var smsList = db.Sms
                .Include("Status")
                .Where(x => x.TimeStamp > startTime)
                .AsEnumerable()
                .Select(x => new
                {
                    Status = x.Status,
                    IntervalStart = DateTime.Now.Date.AddHours(x.TimeStamp.Hour)
                })
                .GroupBy(x => x.IntervalStart)
                .Select(grp => new SummaryInterval()
                {
                    IntervalName = grp.Key.ToString(),
                    IntervalData = grp.GroupBy(x => x.Status.Name)
                                            .Select(x => new SummaryStatusCount
                                            {
                                                StatusName = x.Key,
                                                StatusCount = x.Count()
                                            })
                                        .ToList(),
                    IntervalStart = grp.Key
                })
                .ToList();

            summary.SummaryIntervalList = smsList;
            return summary;
        }
    }

    public class HourSummaryFactory : ISummaryFactory
    {
        public async Task<Summary> CreateAsync(ISendSMSHostContext db)
        {
            Summary summary = new Summary();

            DateTime nu = DateTime.Now;
            DateTime startTime = nu.Date.AddHours(nu.Hour - 1)
                                        .AddMinutes(nu.Minute);

            var smsList = db.Sms
                .Include("Status")
                .Where(x => x.TimeStamp > startTime)
                .AsEnumerable()
                .Select(x => new
                {
                    Status = x.Status,
                    IntervalStart = x.TimeStamp.Date
                                        .AddHours(x.TimeStamp.Hour)
                                        .AddMinutes(x.TimeStamp.Minute)
                })
                .GroupBy(x => x.IntervalStart)
                .Select(grp => new SummaryInterval()
                {
                    IntervalName = grp.Key.ToString(),
                    IntervalData = grp.GroupBy(x => x.Status.Name)
                                            .Select(x => new SummaryStatusCount
                                            {
                                                StatusName = x.Key,
                                                StatusCount = x.Count()
                                            })
                                        .ToList(),
                    IntervalStart = grp.Key
                })
                .ToList();

            summary.SummaryIntervalList = smsList;
            return summary;
        }
    }
}