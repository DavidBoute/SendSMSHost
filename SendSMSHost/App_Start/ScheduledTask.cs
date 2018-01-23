using AutoMapper;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
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
    // Voert taken iedere x seconden uit
    // https://stackoverflow.blog/2008/07/18/easy-background-tasks-in-aspnet/
    public class ScheduledTask
    {
        private CacheItemRemovedCallback _onCacheRemove = null;
        private IHubContext _signalRContext;

        public void Start()
        {
            AddTask("EnqueueSmsToSend", 10);
            AddTask("ImportSms", 10);
        }

        public void CacheItemRemoved(string taskName, object taskTime, CacheItemRemovedReason r)
        {
            if (taskName == "EnqueueSmsToSend")
            {
                EnqueueSmsToSend(taskName, taskTime);
            }
            else if (taskName == "ImportSms")
            {
                ImportSms(taskName, taskTime);
            }
        }

        /// <summary>
        /// Zorgt er voor dat er  steeds 5 sms'en zijn om te verwerken
        /// Status Queued of Pending
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="taskTime"></param>
        private async void EnqueueSmsToSend(string taskName, object taskTime)
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

                        _signalRContext.Clients.All.notifyChangeToPage(new SmsDTOWithOperation { SmsDTO = smsDTO, Operation = "PUT" });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            // re-add our task so it recurs
            AddTask(taskName, Convert.ToInt32(taskTime));
        }

        /// <summary>
        /// Leest periodiek de tabel ImportSms uit en voegt sms'en en nummers toe aan database
        /// Verwittigt via Signal R de andere applicaties
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="taskTime"></param>
        private async void ImportSms(string taskName, object taskTime)
        {
            // nieuwe context opvragen
            var db = new SendSMSHostContext();

            // kijken als er reccords in de tabel ImportSms zijn
            // en toevoegen aan genormaliseerde tabellen.

            if (db.ImportSms.Count() > 0)
            {
                Status statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");

                var smsToImportList = db.ImportSms.ToList();
                foreach (var smsToImport in smsToImportList)
                {

                    // kijken of nummer al in gebruik is
                    Contact contact = db.Contacts.SingleOrDefault(x => x.Number == smsToImport.ContactNumber);
                    if (contact == null) // nieuw contact maken
                    {
                        contact = new Contact
                        {
                            Id = Guid.NewGuid(),
                            FirstName = (smsToImport.ContactFirstName != String.Empty ?
                                            smsToImport.ContactFirstName : smsToImport.ContactNumber),
                            LastName = smsToImport.ContactLastName,
                            Number = smsToImport.ContactNumber,
                            IsAnonymous = (smsToImport.ContactFirstName != String.Empty)
                        };

                        db.Contacts.Add(contact);
                        try
                        {
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            throw;
                        }
                    }

                    Sms sms = new Sms
                    {
                        Id = Guid.NewGuid(),
                        ContactId = contact.Id,
                        Message = smsToImport.Message,
                        StatusId = statusCreated.Id,
                        TimeStamp = DateTime.Now,
                    };

                    db.Sms.Add(sms);
                    db.ImportSms.Remove(smsToImport);
                }

                try
                {
                    await db.SaveChangesAsync();
                    _signalRContext.Clients.All.notifyChangeToPage(new SmsDTOWithOperation { SmsDTO = null, Operation = "POST" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            // re-add our task so it recurs
            AddTask(taskName, Convert.ToInt32(taskTime));
        }

        private void AddTask(string name, int seconds)
        {
            _onCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, _onCacheRemove);
        }

        public ScheduledTask()
        {
            _signalRContext = GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>();
        }

    }
}