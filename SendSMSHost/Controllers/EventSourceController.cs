using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SendSMSHost.Controllers
{
    public class EventSourceController : Controller
    {
        // SSE
        // http://www.c-sharpcorner.com/blogs/server-side-events-in-asp-net-mvc
        // https://stackoverflow.com/questions/35631938/server-sent-events-eventsource-with-standard-asp-net-mvc-causing-error

        public class PingData
        {
            public string ClientName { get; set; }
            public string Method { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;
        }

        static ConcurrentQueue<PingData> pings = new ConcurrentQueue<PingData>();

        public void Ping(string clientName, string method)
        {
            pings.Enqueue(new PingData { ClientName = clientName, Method = method });
        }

        public void Message()
        {
            Response.ContentType = "text/event-stream";
            bool hasConnection = true;

            do
            {
                try
                {
                    if (pings.TryDequeue(out PingData nextPing))
                    {
                        Response.Write("data:" + JsonConvert.SerializeObject(nextPing, Formatting.None) + "\n\n");
                        Response.Flush();

                        // extra event triggeren, anders komt javascript 1 achter
                        Response.Write("data:" + "\n\n");
                        Response.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(DateTime.Now + ": " + ex.Message);
                    hasConnection = false;
                }

                Thread.Sleep(1000);
            } while (hasConnection);

            Response.Close();
        }
    }
}