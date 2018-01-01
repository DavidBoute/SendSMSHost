using AutoMapper;
using Microsoft.AspNet.SignalR;
using SendSMSHost.Models;
using SendSMSHost.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace SendSMSHost.App_Start
{
    // Periodic tasks in ASP.Net 
    // https://stackoverflow.blog/2008/07/18/easy-background-tasks-in-aspnet/
    public class ScheduledTask
    {
        private CacheItemRemovedCallback OnCacheRemove = null;

        public void Start()
        {
            AddTask("EnqueueSmsToSend", 10);
        }

        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public async void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            // nieuwe context opvragen
            var db = new SendSMSHostContext();

            // De lijst van sms'en met de status Queued aanvullen tot 5
            // aanpassen in database + clients verwittigen

            var createdSmsList = db.Status.Include("sms").FirstOrDefault(y => y.Name == "Created").Sms.OrderBy(z => z.TimeStamp).ToList();
            var queuedSmsList = db.Status.Include("sms").FirstOrDefault(y => y.Name == "Queued").Sms.ToList();
            if (createdSmsList.Count() > 0
                && queuedSmsList.Count() < 5)
            {
                var signalRContext = GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>();

                var changeToQueuedSmsList = createdSmsList.Take(
                    Math.Min(5 - queuedSmsList.Count(), createdSmsList.Count()));
                Status statusQueued = db.Status.FirstOrDefault(x => x.Name == "Queued");

                var queuedSmsDTOList = new List<SmsDTO>();
                foreach (var sms in changeToQueuedSmsList)
                {
                    sms.Status = statusQueued;
                    SmsDTO smsDTO = Mapper.Map<SmsDTO>(sms);

                    try
                    {
                        await db.SaveChangesAsync();

                        signalRContext.Clients.All.notifyChangeToPage(new SmsDTOWithClient { Client = "Server", Operation = "PUT", SmsDTO = smsDTO });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }

            // re-add our task so it recurs
            AddTask(k, Convert.ToInt32(v));
        }
    }
}