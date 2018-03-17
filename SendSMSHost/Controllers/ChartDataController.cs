using Microsoft.AspNet.SignalR;
using SendSMSHost.Models;
using SendSMSHost.Models.Factory;
using SendSMSHost.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SendSMSHost.Controllers
{
    public class ChartDataController : ApiController
    {
        private SendSMSHostContext db = new SendSMSHostContext();
        private IHubContext _signalRContext;

        // GET: api/ChartData/Forever
        [HttpGet]
        [ActionName("Forever")]
        [ResponseType(typeof(ChartData))]
        public IHttpActionResult GetForeverChartData()
        {
            IChartDataFactory chartDataFactory = new ForeverChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db);

            return Ok(chartdata);
        }

        // GET: api/ChartData/Week
        [HttpGet]
        [ActionName("Week")]
        [ResponseType(typeof(ChartData))]
        public IHttpActionResult GetWeekChartData()
        {
            IChartDataFactory chartDataFactory = new WeekChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db);

            return Ok(chartdata);
        }

        // GET: api/ChartData/Day
        [HttpGet]
        [ActionName("Day")]
        [ResponseType(typeof(ChartData))]
        public IHttpActionResult GetDayChartData()
        {
            IChartDataFactory chartDataFactory = new DayChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db);

            return Ok(chartdata);
        }

        // GET: api/ChartData/Hour
        [HttpGet]
        [ActionName("Hour")]
        [ResponseType(typeof(ChartData))]
        public IHttpActionResult GetHourChartData()
        {
            IChartDataFactory chartDataFactory = new HourChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db);

            return Ok(chartdata);
        }

        public ChartDataController()
        {
            _signalRContext = GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>();
        }
    }
}
