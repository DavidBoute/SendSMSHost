using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SendSMSHost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult IndexVue()
        {
            return View();
        }

        // SSE - http://www.c-sharpcorner.com/blogs/server-side-events-in-asp-net-mvc
        public void Message()
        {
            Response.ContentType = "text/event-stream";

            DateTime startDate = DateTime.Now;
            while (startDate.AddMinutes(1) > DateTime.Now)
            {
                Response.Write(string.Format("data: {0}\n\n", DateTime.Now.ToString()));
                Response.Flush();

                System.Threading.Thread.Sleep(1000);
            }

            Response.Close();
        }
    }
}
