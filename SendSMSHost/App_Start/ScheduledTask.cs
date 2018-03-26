using AutoMapper;
using FluentScheduler;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SendSMSHost.Models;
using SendSMSHost.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

namespace SendSMSHost
{
    // https://github.com/fluentscheduler/FluentScheduler
    public class ScheduledTask
    {
        public class TaskRegistry : Registry
        {
            public TaskRegistry()
            {
                Schedule<EnqueueSmsToSend>().ToRunEvery(1).Seconds();
                Schedule<ImportSms>().ToRunEvery(10).Seconds();
            }
        }

        public class EnqueueSmsToSend : IJob, IRegisteredObject
        {
            private IHubContext _signalRContext
                    = GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>();

            private readonly object _lock = new object();
            private bool _shuttingDown;

            public EnqueueSmsToSend()
            {
                // Register this job with the hosting environment.
                // Allows for a more graceful stop of the job, in the case of IIS shutting down.
                HostingEnvironment.RegisterObject(this);
            }

            public void Execute()
            {
                try
                {
                    lock (_lock)
                    {
                        if (_shuttingDown)
                            return;

                        // nieuwe dbContext opvragen
                        using (SendSMSHostContext db = new SendSMSHostContext())
                        {
                            // De lijst van sms'en met de status Queued of Pending aanvullen tot batchSize
                            // aanpassen in database + clients verwittigen

                            Debug.WriteLine($"[{DateTime.Now}] Checking sms to queue");

                            int batchSize = 5;

                            Status statusQueued = db.Status.FirstOrDefault(x => x.Name == "Queued");
                            Status statusPending = db.Status.FirstOrDefault(x => x.Name == "Pending");
                            Status statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");

                            int queuedCount = db.Sms
                                                    .Count(x => x.StatusId == statusQueued.Id
                                                            || x.StatusId == statusPending.Id);
                            int createdCount = db.Sms
                                                    .Count(x => x.StatusId == statusCreated.Id);

                            int amountToChange =  0;
                            if (batchSize > queuedCount)
                            {
                                amountToChange = Math.Min(batchSize - queuedCount, createdCount);
                            }

                            if (amountToChange > 0)
                            {
                                Debug.WriteLine($"[{DateTime.Now}] Enqueuing {amountToChange} sms");

                                var changeToQueuedSmsList = db.Sms
                                                                .Where(x => x.StatusId == statusCreated.Id)
                                                                .OrderBy(z => z.TimeStamp)
                                                                .Take(amountToChange)
                                                                .ToList();

                                var queuedSmsDTOList = new List<SmsDTO>();
                                foreach (var sms in changeToQueuedSmsList)
                                {
                                    sms.Status = statusQueued;
                                    SmsDTO smsDTO = Mapper.Map<SmsDTO>(sms);

                                    try
                                    {
                                        db.SaveChanges();

                                        ServerSentEventsHub.NotifyChange(_signalRContext,
                                            new SmsDTOWithOperation { SmsDTO = smsDTO, Operation = "PUT" });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // Always unregister the job when done.
                    HostingEnvironment.UnregisterObject(this);
                }
            }

            public void Stop(bool immediate)
            {
                // Locking here will wait for the lock in Execute to be released until this code can continue.
                lock (_lock)
                {
                    _shuttingDown = true;
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        public class ImportSms : IJob, IRegisteredObject
        {
            private IHubContext _signalRContext
                    = GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>();

            private readonly object _lock = new object();
            private bool _shuttingDown;

            public ImportSms()
            {
                // Register this job with the hosting environment.
                // Allows for a more graceful stop of the job, in the case of IIS shutting down.
                HostingEnvironment.RegisterObject(this);
            }

            public void Execute()
            {
                try
                {
                    lock (_lock)
                    {
                        if (_shuttingDown)
                            return;

                        // nieuwe context opvragen
                        using (var db = new SendSMSHostContext())
                        {
                            // kijken als er reccords in de tabel ImportSms zijn
                            // en toevoegen aan genormaliseerde tabellen.
                            Debug.WriteLine($"[{DateTime.Now}] Checking sms to import");

                            int importSmsCount = db.ImportSms.Count();
                            if (importSmsCount > 0)
                            {
                                Debug.WriteLine($"[{DateTime.Now}] Importing {importSmsCount} sms");
                            
                                Status statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");

                                var smsToImportList = db.ImportSms.ToList();
                                foreach (var smsToImport in smsToImportList)
                                {
                                    // kijken of nummer al in gebruik is
                                    Contact contact = db.Contacts.SingleOrDefault(x => x.Number == smsToImport.ContactNumber);
                                    if (contact == null) // nieuw contact maken
                                    {
                                        Debug.WriteLine($"[{DateTime.Now}] Creating new contact");
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
                                            db.SaveChanges();
                                            Debug.WriteLine($"[{DateTime.Now}] Contact created: {contact.FirstName} {contact.LastName}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex.Message);
                                            throw;
                                        }
                                    }

                                    Debug.WriteLine($"[{DateTime.Now}] Creating new sms");
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
                                    db.SaveChanges();

                                    ServerSentEventsHub.NotifyChange(_signalRContext,
                                        new SmsDTOWithOperation { SmsDTO = null, Operation = "POST" });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    throw;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // Always unregister the job when done.
                    HostingEnvironment.UnregisterObject(this);
                }
            }

            public void Stop(bool immediate)
            {
                // Locking here will wait for the lock in Execute to be released until this code can continue.
                lock (_lock)
                {
                    _shuttingDown = true;
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }
    }
}