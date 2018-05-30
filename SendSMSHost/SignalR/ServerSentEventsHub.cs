﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SendSMSHost.Models;
using SendSMSHost.Models.Factory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SendSMSHost.SignalR
{
    public class ServerSentEventsHub : Hub
    {
        private SendSMSHostContext db;

        #region Request methods // Clients vragen data aan

        #region Sms

        /// <summary>
        /// Stuur een boodschap naar alle clients, om als popup te tonen
        /// </summary>
        /// <param name="message">de boodschap</param>
        public void RequestDisplayMessage(string message)
        {
            Clients.All.displayMessage(message);
        }

        /// <summary>
        /// Stuurt een lijst van SmsDTO's naar de caller
        /// </summary>
        /// <param name="includeCreated">worden de smsDTO's met status Created mee gestuurd</param>
        public void RequestSmsList(bool includeCreated)
        {
            List<SmsDTO> smsDTOList;

            if (!includeCreated)
            {
                var statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");
                smsDTOList = db.Sms
                            .Where(x => x.StatusId != statusCreated.Id)
                            .OrderBy(x => x.TimeStamp)
                            .AsEnumerable()
                            .Select(x => new SmsDTO(x))
                            .ToList();
            }
            else
            {
                smsDTOList = db.Sms
                            .OrderBy(x => x.TimeStamp)
                            .AsEnumerable()
                            .Select(x => new SmsDTO(x))
                            .ToList();
            }

            Clients.Caller.getSmsList(smsDTOList);
        }

        /// <summary>
        /// Stuurt een lijst van StatusDTO's naar de caller
        /// </summary>
        public void RequestStatusList()
        {
            List<StatusDTO> statusDTOList;

            statusDTOList = db.Status
                            .AsEnumerable()
                            .Select(x => new StatusDTO(x))
                            .ToList();

            Clients.Caller.getStatusList(statusDTOList);
        }

        /// <summary>
        /// Stuurt een lijst van StatusDTO's naar de caller
        /// </summary>
        public void RequestContactList()
        {
            List<ContactDTO> contactDTOList;

            contactDTOList = db.Contacts
                            .Where(x => !x.IsAnonymous)
                            .AsEnumerable()
                            .Select(x => new ContactDTO(x))
                            .ToList();


            Clients.Caller.getContactList(contactDTOList);
        }

        /// <summary>
        /// Stuurt een de actuele status van het sturen van berichten naar de caller
        /// </summary>
        public void RequestSendStatus()
        {
            Clients.Others.getSendStatus();
        }

        #endregion      

        #region Wijzigingen

        /// <summary>
        /// Maakt een Sms in de database.
        /// </summary>
        /// <param name="smsDTO">de te maken sms</param>
        public async Task RequestCreateSms(SmsDTO smsDTO)
        {
            // Indien ContactId == null dan opzoeken of nieuw contact voorzien
            if (String.IsNullOrWhiteSpace(smsDTO.ContactId))
            {
                // kijken of nummer al in gebruik is
                Contact contact = db.Contacts
                                    .SingleOrDefault(x => x.Number == smsDTO.ContactNumber);
                if (contact == null) // nieuw contact maken
                {
                    contact = new Contact
                    {
                        Id = Guid.NewGuid(),
                        FirstName = smsDTO.ContactNumber,
                        LastName = "",
                        Number = smsDTO.ContactNumber,
                        IsAnonymous = true
                    };

                    db.Contacts.Add(contact);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        throw;
                    }
                }
                smsDTO.ContactId = contact.Id.ToString();
                smsDTO.ContactFirstName = contact.FirstName;
                smsDTO.ContactLastName = contact.LastName;
            }

            Sms sms = new Sms(smsDTO);

            sms.Id = Guid.NewGuid();
            sms.TimeStamp = DateTime.Now;
            sms.Status = await db.Status
                                .SingleOrDefaultAsync(x => x.Name == "Created");
            db.Sms.Add(sms);

            try
            {
                await db.SaveChangesAsync();

                smsDTO = new SmsDTO(await db.Sms
                                        .SingleOrDefaultAsync(x => x.Id == sms.Id));


                UpdateLog(db, smsDTO, "POST");
                NotifyCreateSms(smsDTO, Clients);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Past een Sms aan in de database.
        /// </summary>
        /// <param name="smsDTO">de aan te passen sms</param>
        public async Task RequestEditSms(SmsDTO smsDTO)
        {
            Sms sms = Sms.FindSmsById(smsDTO.Id);
            sms.CopyFromSmsDTO(smsDTO);

            db.Set<Sms>().Attach(sms);
            db.Entry(sms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();

                UpdateLog(db, smsDTO, "PUT");
                NotifyEditSms(smsDTO, Clients);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Verwijdert een Sms in de database.
        /// </summary>
        /// <param name="smsDTO">de te verwijderen sms</param>
        public async Task RequestDeleteSms(SmsDTO smsDTO)
        {
            Sms sms = await db.Sms.FindAsync(Guid.Parse(smsDTO.Id));
            if (sms != null)
            {
                db.Sms.Remove(sms);
                try
                {
                    await db.SaveChangesAsync();

                    UpdateLog(db, smsDTO, "DELETE");
                    NotifyDeleteSms(smsDTO, Clients);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Importeert een lijst van Sms'en in tblImportSms
        /// </summary>
        /// <param name="smsDTOList">de te importeren sms</param>
        public async Task RequestCreateSmsBulk(List<SmsDTO> smsDTOList)
        {
            IEnumerable<ImportSms> smsImportList = smsDTOList.Select(x => new ImportSms()
            {
                ContactNumber = x.ContactNumber,
                Message = x.Message
            })
            .AsEnumerable();

            db.ImportSms.AddRange(smsImportList);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        #endregion

        #region ChartData

        /// <summary>
        /// Stuurt een ForeverChartData naar de caller
        /// </summary>
        public void RequestForeverChart(bool includeDeleted = false)
        {
            IChartDataFactory chartDataFactory = new ForeverChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db, includeDeleted);

            Clients.Caller.notifyChangeForeverChart(chartdata);
        }

        /// <summary>
        /// Stuurt een WeekChartData naar de caller
        /// </summary>
        public void RequestWeekChart(bool includeDeleted = false)
        {
            IChartDataFactory chartDataFactory = new WeekChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db, includeDeleted);

            Clients.Caller.notifyChangeWeekChart(chartdata);
        }

        /// <summary>
        /// Stuurt een DayChartData naar de caller
        /// </summary>
        public void RequestDayChart(bool includeDeleted = false)
        {
            IChartDataFactory chartDataFactory = new DayChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db, includeDeleted);

            Clients.Caller.notifyChangeDayChart(chartdata);
        }

        /// <summary>
        /// Stuurt een HourChartData naar de caller
        /// </summary>
        public void RequestHourChart(bool includeDeleted = false)
        {
            IChartDataFactory chartDataFactory = new HourChartDataFactory();
            ChartData chartdata = chartDataFactory?.CreateChartData(db, includeDeleted);

            Clients.Caller.notifyChangeHourChart(chartdata);
        }

        #endregion

        #endregion

        #region Notify methods // Brengen clients op de hoogte van wijzigingen

        #region Sms

        public void NotifyCreateSms(SmsDTO smsDTO, IHubCallerConnectionContext<dynamic> clients)
        {
            clients.All.notifyCreateSms(smsDTO);
            clients.All.notifyChangeToCharts();
        }

        public void NotifyEditSms(SmsDTO smsDTO, IHubCallerConnectionContext<dynamic> clients)
        {
            clients.All.notifyEditSms(smsDTO);
            clients.All.notifyChangeToCharts();
        }

        public void NotifyDeleteSms(SmsDTO smsDTO, IHubCallerConnectionContext<dynamic> clients)
        {
            clients.All.notifyDeleteSms(smsDTO);
            clients.All.notifyChangeToCharts();
        }

        /// <summary>
        /// Brengt andere clients op de hoogte dat een bepaalde SmsDTO aangepast is
        /// Zorgt voor volledige herinladen lijst
        /// </summary>
        /// <param name="smsDTOWithClient">de aangepaste SmsDTO met bewerkingsgegevens</param>
        public void NotifyChange(SmsDTO smsDTO, string operation)
        {
            Clients.Others.notifyChangeToSmsList();
            Clients.All.notifyChangeToCharts();

            UpdateLog(db, smsDTO, operation);
        }

        /// <summary>
        /// Brengt andere clients op de hoogte dat een toggle om berichten te sturen aangepast is
        /// </summary>
        /// <param name="sendStatus">de actuele status</param>
        public void NotifySendStatus(bool sendStatus)
        {
            Clients.Others.notifySendStatus(sendStatus);
        }

        /// <summary>
        /// Algemene methode om clients te verwittigen van wijzigingen 
        /// Zorgt voor volledige herinladen lijst
        /// Voor gebruik door server
        /// </summary>
        /// <param name="hubContext"></param>
        /// <param name="smsDTO"></param>
        /// <param name="operation"></param>
        public static void NotifyChange(IHubContext hubContext, SmsDTO smsDTO, string operation)
        {
            using (SendSMSHostContext db = new SendSMSHostContext())
            {
                UpdateLog(db, smsDTO, operation);
            }

            switch (operation)
            {
                case "POST":
                    hubContext.Clients.All.notifyCreateSms(smsDTO);
                    break;
                case "PUT":
                    hubContext.Clients.All.notifyEditSms(smsDTO);
                    break;
                case "DELETE":
                    hubContext.Clients.All.notifyDeleteSms(smsDTO);
                    break;
                default:
                    hubContext.Clients.All.notifyChangeToSmsList();
                    break;
            }

            hubContext.Clients.All.notifyChangeToCharts();
        }

        #endregion

        #endregion

        #region Send methods // Zorgen er voor dat app begint te sturen

        /// <summary>
        /// Geeft aan telefoon de opdracht om een sms te zenden, ongeacht de huidige status
        /// </summary>
        /// <param name="smsID">Id van de sms</param>
        public async Task SendSelectedSms(Guid smsId)
        {
            var smsDTO = new SmsDTO(await db.Sms.SingleOrDefaultAsync(x => x.Id == smsId));
            Clients.Others.sendSelectedSms(smsDTO);
        }

        // TODO: automatisch zenden uitschakelen

        /// <summary>
        /// Start het automatisch zenden van Pending sms'en
        /// </summary>
        public void ToggleSendPending(bool startSend)
        {
            Clients.Others.toggleSendPending(startSend);
        }

        #endregion

        #region Logging methods // Zorgen dat bewerkingen bijgehouden worden

        public static void UpdateLog(SendSMSHostContext db, SmsDTO smsDTO, string operation)
        {
            db.Log.Add(new Log
            {
                SmsId = smsDTO.Id,
                Operation = operation,
                Timestamp = DateTime.Now,
                StatusName = smsDTO.StatusName
            });

            db.SaveChanges();
        }

        #endregion

        public ServerSentEventsHub()
        {
            db = new SendSMSHostContext();
        }
    }
}