﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(SendSMSHost.ServerSentEventsStartup))]

namespace SendSMSHost
{
    public class ServerSentEventsStartup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            // Any connection or hub wire up and configuration should go here
            app.MapSignalR(); // standaardinstellingen

        }
    }
}
