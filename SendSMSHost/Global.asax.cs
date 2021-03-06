﻿using FluentScheduler;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNet.SignalR;
using SendSMSHost.Models;
using System.Data.Entity;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SendSMSHost
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DisableApplicationInsightsOnDebug();

            //Database.SetInitializer(new YourInititalizer());
            //var dbContext = new SendSMSHostContext();
            //dbContext.Database.Initialize(force: true);

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            JobManager.Initialize(new ScheduledTask.TaskRegistry());
        }

        /// <summary>
        /// Disables the application insights locally.
        /// </summary>
        [Conditional("DEBUG")]
        private static void DisableApplicationInsightsOnDebug()
        {
            TelemetryConfiguration.Active.DisableTelemetry = true;
        }
    }
}
