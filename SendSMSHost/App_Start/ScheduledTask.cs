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

        static bool IsBusyImporting { get; set; }

        public class EnqueueSmsToSend : IJob, IRegisteredObject
        {
            const int BATCHSIZE = 5;

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

                        if (IsBusyImporting)
                            return;

                        // nieuwe dbContext opvragen
                        using (SendSMSHostContext db = new SendSMSHostContext())
                        {
                            // De lijst van sms'en met de status Queued of Pending aanvullen tot batchSize
                            // aanpassen in database + clients verwittigen

                            Debug.WriteLine($"[{DateTime.Now}] Checking sms to queue");

                            Status statusQueued = db.Status.FirstOrDefault(x => x.Name == "Queued");
                            Status statusPending = db.Status.FirstOrDefault(x => x.Name == "Pending");
                            Status statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");

                            int queuedCount = db.Sms
                                                    .Count(x => x.StatusId == statusQueued.Id
                                                            || x.StatusId == statusPending.Id);
                            int createdCount = db.Sms
                                                    .Count(x => x.StatusId == statusCreated.Id);

                            int amountToChange = 0;
                            if (BATCHSIZE > queuedCount)
                            {
                                amountToChange = Math.Min(BATCHSIZE - queuedCount, createdCount);
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
                                    sms.TimeStamp = DateTime.Now;
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

                        if (IsBusyImporting)
                            return;

                        IsBusyImporting = true;
                        Debug.WriteLine($"[{DateTime.Now}] Checking sms to import");

                        // nieuwe context opvragen
                        using (var db = new SendSMSHostContext())
                        {
                            // kijken als er reccords in de tabel ImportSms zijn
                            // en toevoegen aan genormaliseerde tabellen.

                            int importSmsCount = db.ImportSms.Count();
                            if (importSmsCount > 0)
                            {
                                Debug.WriteLine($"[{DateTime.Now}] Importing {importSmsCount} sms");

                                Status statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");

                                // eerst alle contacten aanmaken
                                var contactNumberList = db.ImportSms
                                                            .Select(x => x.ContactNumber)
                                                            .Distinct()
                                                            .Where(x => !db.Contacts
                                                                            .Select(y => y.Number)
                                                                            .Contains(x))
                                                            .AsEnumerable()
                                                            .Select(x => new Contact
                                                            {
                                                                Id = Guid.NewGuid(),
                                                                Number = x,
                                                                IsAnonymous = true
                                                            });


                                db.Contacts.AddRange(contactNumberList);
                                int contactNumberListCount = contactNumberList.Count();

                                try
                                {
                                    db.SaveChanges();
                                    Debug.WriteLine($"[{DateTime.Now}] Created {contactNumberListCount} new contacts");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    throw ex;
                                }

                                // dan alle sms'en aanmaken
                                var smsToImport = db.ImportSms
                                                    .AsEnumerable()
                                                    .Select(x => new Sms
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        Contact = db.Contacts
                                                                        .SingleOrDefault(y => y.Number == x.ContactNumber),
                                                        Message = x.Message,
                                                        Status = statusCreated,
                                                        TimeStamp = DateTime.Now,
                                                    });

                                db.Sms.AddRange(smsToImport);
                                var smsToImportList = smsToImport.ToList();

                                // dan ImportSms leeg maken
                                db.ImportSms.RemoveRange(db.ImportSms);

                                try
                                { 
                                    db.SaveChanges();
                                    Debug.WriteLine($"[{DateTime.Now}] Created {importSmsCount} new sms");

                                    foreach (Sms s in smsToImportList)
                                    {
                                        ServerSentEventsHub.NotifyChange(_signalRContext,
                                            new SmsDTOWithOperation { SmsDTO = Mapper.Map<SmsDTO>(s), Operation = "POST" });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                    throw ex;
                                }
                            }
                        }
                        IsBusyImporting = false;
                        Debug.WriteLine($"[{DateTime.Now}] Importing finished");
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