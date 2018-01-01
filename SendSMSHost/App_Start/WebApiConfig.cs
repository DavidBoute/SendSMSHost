using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace SendSMSHost
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            // Gefilterde lijst van sms'en enkel voor telefoon, zonder created
            config.Routes.MapHttpRoute(
                name: "GetSmsListPhone",
                routeTemplate: "api/smsphone",
                defaults: new { controller = "Sms", action = "GetSmsPhone" }
            );

            // gewone lijst van sms'en
            config.Routes.MapHttpRoute(
                name: "GetSmsList",
                routeTemplate: "api/sms",
                defaults: new { controller = "Sms", action = "GetSms" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


        }
    }
}
