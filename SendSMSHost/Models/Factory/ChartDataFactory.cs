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
    }

    public class ForeverChartDataFactory : IChartDataFactory
    {
        public ChartData CreateChartData(ISendSMSHostContext db)
        {
            var data = db.Status
                        .Include("Sms")
                        .Select(x => new
                        {
                            Label = x.Name,
                            Data = x.Sms.Count(),
                            BackgroundColor = x.DefaultColorHex
                        })
                        .AsEnumerable()
                        .Select(d => new DataSet
                        {
                            Label = d.Label,
                            Data = new int[] { d.Data },
                            BackgroundColor = d.BackgroundColor
                        })
                        .ToArray();

            ChartData chartData = new ChartData
            {
                Labels = null,
                Datasets = data
            };

            return chartData;
        }
    }

    public class WeekChartDataFactory : IChartDataFactory
    {
        public ChartData CreateChartData(ISendSMSHostContext db)
        {
            List<DateTime> dateList = new List<DateTime>();
            for (int i = 6; i >= 0; i--)
            {
                dateList.Add(DateTime.Today.AddDays(-i));
            }

            var data = db.Status
            .Include("Sms")
            .Select(d => new
            {
                Label = d.Name,
                Data = dateList
                        .OrderBy(dt => dt)
                        .Select(date => d.Sms
                                        .Count(x => date <= x.TimeStamp
                                                && x.TimeStamp < DbFunctions.AddDays(date, 1))),
                BackgroundColor = d.DefaultColorHex
            })
            .AsEnumerable()
            .Select(d => new DataSet
            {
                Label = d.Label,
                Data = d.Data.ToArray(),
                BackgroundColor = d.BackgroundColor
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
        public ChartData CreateChartData(ISendSMSHostContext db)
        {
            List<DateTime> hourList = new List<DateTime>();
            for (int i = 0; i <= 23; i++)
            {
                hourList.Add(DateTime.Today.AddHours(i));
            }

            var data = db.Status
                        .Include("Sms")
                        .Select(d => new
                        {
                            Label = d.Name,
                            Data = hourList
                                    .OrderBy(dt => dt)
                                    .Select(h => d.Sms
                                                .Count(x => h <= x.TimeStamp
                                                        && x.TimeStamp < DbFunctions.AddHours(h, 1))),
                            BackgroundColor = d.DefaultColorHex
                        })
                        .AsEnumerable()
                        .Select(d => new DataSet
                        {
                            Label = d.Label,
                            Data = d.Data.ToArray(),
                            BackgroundColor = d.BackgroundColor
                        })
                        .ToArray();

            ChartData chartData = new ChartData
            {
                Labels = hourList.Select(d => d.ToString("hh:mm")).ToArray(),
                Datasets = data
            };

            return chartData;
        }
    }

    public class HourChartDataFactory : IChartDataFactory
    {
        public ChartData CreateChartData(ISendSMSHostContext db) { return new ChartData(); }
    }
}