using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SendSMSHost.Models;

namespace SendSMSHost.SignalR
{
    public class ServerSentEventsHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.sendMessage(message);
        }

        public void NotifyChange(SmsDTOWithClient smsDTOWithClient)
        {
            Clients.All.notifyChangeToPage(smsDTOWithClient);
        }
    }
}