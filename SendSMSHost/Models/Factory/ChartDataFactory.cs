using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SendSMSHost.Models.Factory
{
    public interface IChartDataFactory
    {
        ChartData CreateChartData(ISendSMSHostContext db);
        ChartData CreateChartData(ISendSMSHostContext db, bool includeDeleted);
    }

    public class ChartData
    {
        [JsonProperty("labels")]
        public string[] Labels { get; set; }

        [JsonProperty("datasets")]
        public DataSet[] Datasets { get; set; }
    }

    public class DataSet
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("data")]
        public int[] Data { get; set; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }

        [JsonProperty("borderWidth")]
        public int BorderWidth { get; set; }
    }

    public class ForeverChartDataFactory : IChartDataFactory
    {
        public ChartData CreateChartData(ISendSMSHostContext db) { return CreateChartData(db, false); }

        public ChartData CreateChartData(ISendSMSHostContext db, bool includeDeleted)
        {
            var lastLogs = db.Log
                            .GroupBy(x => x.SmsId)
                            .Select(y => y.OrderByDescending(z => z.Timestamp).FirstOrDefault());

            if (!includeDeleted)
            {
                lastLogs = lastLogs.Where(z => z.Operation != "DELETE");
            };

            var data = db.Status
                            .Select(s => new
                            {
                                Label = s.Name,
                                Data = lastLogs.Count(l => l.StatusName == s.Name),
                                BackgroundColor = s.DefaultColorHex
                            })
                            .AsEnumerable()
                            .Select(d => new DataSet
                            {
                                Label = d.Label,
                                Data = new int[] { d.Data },
                                BackgroundColor = d.BackgroundColor,
                                BorderColor = "#868E96",
                                BorderWidth = 1
                            })
                            .ToArray();

            ChartData chartData = new ChartData
            {
                Labels = data.Select(x => x.Label).ToArray(),
                Datasets = data
            };

            return chartData;
        }
    }

    public class WeekChartDataFactory : IChartDataFactory
    {
        public ChartData CreateChartData(ISendSMSHostContext db) { return CreateChartData(db, false); }

        public ChartData CreateChartData(ISendSMSHostContext db, bool includeDeleted)
        {
            List<DateTime> dateList = new List<DateTime>();
            for (int i = 6; i >= 0; i--)
            {
                dateList.Add(DateTime.Today.AddDays(-i));
            }

            var lastLogs = db.Log
                            .Where(x => x.Timestamp >= dateList.Min())
                            .GroupBy(x => x.SmsId)
                            .Select(y => y.OrderByDescending(z => z.Timestamp).FirstOrDefault());

            if (!includeDeleted)
            {
                lastLogs = lastLogs.Where(z => z.Operation != "DELETE");
            };

            var data = db.Status
                            .Select(s => new
                            {
                                Label = s.Name,
                                Data = dateList
                                        .OrderBy(dt => dt)
                                        .Select(date => lastLogs.Count(x => date <= x.Timestamp
                                                                && x.Timestamp < DbFunctions.AddDays(date, 1)
                                                                && x.StatusName == s.Name)),
                                BackgroundColor = s.DefaultColorHex
                            })
                            .AsEnumerable()
                            .Select(d => new DataSet
                            {
                                Label = d.Label,
                                Data = d.Data.ToArray(),
                                BackgroundColor = d.BackgroundColor,
                                BorderColor = "#868E96",
                                BorderWidth = 1
                            })
                            .ToArray();

            ChartData chartData = new ChartData
            {
                Labels = dateList.Select(d => d.ToString("ddd dd/MM")).ToArray(),
                Datasets = data
            };

            return chartData;
        }
    }

    public class DayChartDataFactory : IChartDataFactory
    {
        const int INTERVAL_HOURS = 1;

        public ChartData CreateChartData(ISendSMSHostContext db) { return CreateChartData(db, false); }

        public ChartData CreateChartData(ISendSMSHostContext db, bool includeDeleted)
        {
            List<DateTime> hourList = new List<DateTime>();
            for (int i = 0; i < (24 / INTERVAL_HOURS); i++)
            {
                hourList.Add(DateTime.Today.AddHours(i * INTERVAL_HOURS));
            }

            var lastLogs = db.Log
                            .Where(x => x.Timestamp >= hourList.Min())
                            .GroupBy(x => x.SmsId)
                            .Select(y => y.OrderByDescending(z => z.Timestamp).FirstOrDefault());

            if (!includeDeleted)
            {
                lastLogs = lastLogs.Where(z => z.Operation != "DELETE");
            };

            var data = db.Status
                            .Select(s => new
                            {
                                Label = s.Name,
                                Data = hourList
                                        .OrderBy(dt => dt)
                                        .Select(h => lastLogs.Count(x => h <= x.Timestamp
                                                                && x.Timestamp < DbFunctions.AddHours(h, INTERVAL_HOURS)
                                                                && x.StatusName == s.Name)),
                                BackgroundColor = s.DefaultColorHex
                            })
                            .AsEnumerable()
                            .Select(d => new DataSet
                            {
                                Label = d.Label,
                                Data = d.Data.ToArray(),
                                BackgroundColor = d.BackgroundColor,
                                BorderColor = "#868E96",
                                BorderWidth = 1
                            })
                            .ToArray();

            ChartData chartData = new ChartData
            {
                Labels = hourList.Select(d => d.ToString("HH:mm")).ToArray(),
                Datasets = data
            };

            return chartData;
        }
    }

    public class HourChartDataFactory : IChartDataFactory
    {
        const int INTERVAL_MINUTES = 5;

        public ChartData CreateChartData(ISendSMSHostContext db) { return CreateChartData(db, false); }

        public ChartData CreateChartData(ISendSMSHostContext db, bool includeDeleted)
        {
            List<DateTime> minuteList = new List<DateTime>();
            DateTime dateHour = DateTime.Today.AddHours(DateTime.Now.Hour);

            for (int i = 0; i < (60 / INTERVAL_MINUTES); i++)
            {
                minuteList.Add(dateHour.AddMinutes(i * INTERVAL_MINUTES));
            }

            var lastLogs = db.Log
                            .Where(x => x.Timestamp >= minuteList.Min())
                            .GroupBy(x => x.SmsId)
                            .Select(y => y.OrderByDescending(z => z.Timestamp).FirstOrDefault());

            if (!includeDeleted)
            {
                lastLogs = lastLogs.Where(z => z.Operation != "DELETE");
            };

            var data = db.Status
                            .Select(s => new
                            {
                                Label = s.Name,
                                Data = minuteList
                                        .OrderBy(dt => dt)
                                        .Select(h => lastLogs.Count(x => h <= x.Timestamp
                                                                && x.Timestamp < DbFunctions.AddMinutes(h, INTERVAL_MINUTES)
                                                                && x.StatusName == s.Name)),
                                BackgroundColor = s.DefaultColorHex
                            })
                            .AsEnumerable()
                            .Select(d => new DataSet
                            {
                                Label = d.Label,
                                Data = d.Data.ToArray(),
                                BackgroundColor = d.BackgroundColor,
                                BorderColor = "#868E96",
                                BorderWidth = 1
                            })
                            .ToArray();

            ChartData chartData = new ChartData
            {
                Labels = minuteList.Select(d => d.ToString("HH:mm")).ToArray(),
                Datasets = data
            };

            return chartData;
        }
    }
}