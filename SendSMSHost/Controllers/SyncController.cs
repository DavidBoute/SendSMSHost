using SendSMSHost.ViewModels;
using System.Collections.Specialized;
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
            // lokaal IP van server opzoeken
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    if (localIP.StartsWith("192.168.") || localIP.StartsWith("10.") || localIP.StartsWith("127.0.0."))
                        break;
                }
            }

            string port = Request.ServerVariables["SERVER_PORT"];  //TODO: port via proxy terugvinden
            //string port = "45455"; // Manuele waarde -> zie Conveyor

            SyncIndexViewModel VM = new SyncIndexViewModel
            {
                Prefix = "http://",
                Host = localIP,
                Port = port,
                Path = "api/"
            };
            return View(VM);
        }
    }
}