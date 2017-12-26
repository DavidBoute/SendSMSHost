using SendSMSHost.ViewModels;
using System.Net;
using System.Net.Sockets;
using System.Web.Mvc;

namespace SendSMSHost.Controllers
{
    public class SyncController : Controller
    {
        // GET: Sync
        public ActionResult Index()
        {
            // IP van server opzoeken
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break; // misschien eerst testen of het een lokaal ip is?
                }
            }


            SyncIndexViewModel VM = new SyncIndexViewModel
            {
                Prefix = "http://",
                Host = localIP,
                Port = "45455",
                Path = "api/"
            };
            return View(VM);
        }
    }
}