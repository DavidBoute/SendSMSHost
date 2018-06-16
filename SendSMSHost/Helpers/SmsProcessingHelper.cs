﻿using Microsoft.AspNet.SignalR;
using SendSMSHost.Models;
using SendSMSHost.Models.Factory;
using SendSMSHost.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace SendSMSHost.Helpers
{
    public class SmsProcessingHelper
    {
        private readonly IHubContext _signalRContext;

        public async void Import()
        {
            // nieuwe context opvragen  
            using (var db = new SendSMSHostContext())
            {
                // kijken als er records in de tabel ImportSms zijn
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
                        await db.SaveChangesAsync();
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
                                        })
                                        .ToList();

                    db.Sms.AddRange(smsToImport);

                    // dan ImportSms leeg maken
                    db.ImportSms.RemoveRange(db.ImportSms);

                    try
                    {
                        await db.SaveChangesAsync();
                        Debug.WriteLine($"[{DateTime.Now}] Created {importSmsCount} new sms");

                        foreach (Sms s in smsToImport)
                        {
                            SmsDTO smsDTO = new SmsDTO(s);
                            await ServerSentEventsHub.NotifyChange(_signalRContext,
                                                            smsDTO: smsDTO,
                                                            operation: "POST");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw ex;
                    }
                }
            }
        }

        public async void Enqueue(int batchsize)
        {
            // nieuwe dbContext opvragen
            using (SendSMSHostContext db = new SendSMSHostContext())
            {
                // De lijst van sms'en met de status Queued of Pending aanvullen tot batchSize
                // aanpassen in database + clients verwittigen

                Status statusQueued = db.Status.FirstOrDefault(x => x.Name == "Queued");
                Status statusPending = db.Status.FirstOrDefault(x => x.Name == "Pending");
                Status statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");

                int queuedCount = statusQueued.Sms.Count();
                int createdCount = statusCreated.Sms.Count();

                int amountToChange = 0;
                if (batchsize > queuedCount)
                {
                    amountToChange = Math.Min(batchsize - queuedCount, createdCount);
                }

                if (amountToChange > 0)
                {
                    Debug.WriteLine($"[{DateTime.Now}] Enqueuing {amountToChange} sms");


                    var changeToQueuedSmsList = statusCreated.Sms
                                                    .OrderBy(z => z.TimeStamp)
                                                    .Take(amountToChange)
                                                    .ToList();

                    var queuedSmsDTOList = new List<SmsDTO>();
                    foreach (var sms in changeToQueuedSmsList)
                    {
                        sms.Status = statusQueued;
                        sms.TimeStamp = DateTime.Now;
                        SmsDTO smsDTO = new SmsDTO(sms);

                        try
                        {
                            await db.SaveChangesAsync();

                            await ServerSentEventsHub.NotifyChange(_signalRContext,
                                                             smsDTO: smsDTO,
                                                             operation: "PUT");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }

        public SmsProcessingHelper(IHubContext context)
        {
            _signalRContext = context;
        }
    }
}