using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.SignalR;
using SendSMSHost.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace SendSMSHost.SignalR
{
    public class ServerSentEventsHub : Hub
    {
        private SendSMSHostContext db;
        
        /// <summary>
        /// Stuur een boodschap naar alle clients, om als popup te tonen
        /// </summary>
        /// <param name="message">de boodschap</param>
        public void Send(string message)
        {
            Clients.All.displayMessage(message);
        }

        /// <summary>
        /// Brengt andere clients op de hoogte dat een bepaalde SmsDTO aangepast is
        /// </summary>
        /// <param name="smsDTOWithClient">de aangepaste SmsDTO met bewerkingsgegevens</param>
        public void NotifyChange(SmsDTOWithOperation smsDTOWithOperation)
        {
            Clients.Others.notifyChangeToPage(smsDTOWithOperation);
            Clients.All.notifyChangeToCharts();

            UpdateLog(db, smsDTOWithOperation);
        }

        /// <summary>
        /// Stuurt een lijst van SmsDTO's naar de caller
        /// </summary>
        /// <param name="includeCreated">worden de smsDTO's met status Created mee gestuurd</param>
        public void RequestSmsList(bool includeCreated)
        {
            List<SmsDTO> smsList;

            if (!includeCreated)
            {
                var statusCreated = db.Status.FirstOrDefault(x => x.Name == "Created");
                smsList = db.Sms
                            .Where(x => x.StatusId != statusCreated.Id)
                            .OrderBy(x => x.TimeStamp)
                            .ProjectTo<SmsDTO>()
                            .ToList();
            }
            else
            {
                smsList = db.Sms
                            .OrderBy(x => x.TimeStamp)
                            .ProjectTo<SmsDTO>()
                            .ToList();
            }

            Clients.Caller.getSmsList(smsList);
        }

        /// <summary>
        /// Stuurt een lijst van StatusDTO's naar de caller
        /// </summary>
        public void RequestStatusList()
        {
            List<StatusDTO> statusDTOList;

            statusDTOList = db.Status
                            .ProjectTo<StatusDTO>()
                            .ToList();

            Clients.Caller.getStatusList(statusDTOList);
        }

        /// <summary>
        /// Past een Sms aan in de database.
        /// </summary>
        /// <param name="smsDTO">de aan te passen sms</param>
        public async Task RequestUpdateSms(SmsDTO smsDTO)
        {
            Sms newSms = Mapper.Map<Sms>(smsDTO);
            db.Set<Sms>().Attach(newSms);
            db.Entry(newSms).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
                Clients.Caller.updateSms(smsDTO);

                SmsDTOWithOperation smsDTOWithOperation = new SmsDTOWithOperation { SmsDTO = smsDTO, Operation = "PUT" };
                Clients.Others.notifyChangeToPage(smsDTOWithOperation);
                Clients.All.notifyChangeToCharts();

                UpdateLog(db, smsDTOWithOperation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    Clients.Caller.deleteSms(smsDTO);
                    SmsDTOWithOperation smsDTOWithOperation = new SmsDTOWithOperation { SmsDTO = smsDTO, Operation = "DELETE" };
                    Clients.Others.notifyChangeToPage(smsDTOWithOperation);
                    Clients.All.notifyChangeToCharts();

                    UpdateLog(db, smsDTOWithOperation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Geeft aan telefoon de opdracht om een sms te zenden, ongeacht de huidige status
        /// </summary>
        /// <param name="smsID">Id van de sms</param>
        public async Task SendSelectedSms(Guid smsId)
        {
            var smsDTO = await db.Sms.ProjectTo<SmsDTO>()
                .SingleOrDefaultAsync(x => x.Id == smsId.ToString());
            Clients.Others.sendSelectedSms(smsDTO);
        }

        /// <summary>
        /// Start het automatisch zenden van Pending sms'en
        /// </summary>
        public void toggleSendPending()
        {
            Clients.Others.toggleSendPending(true);
        }

        public static void NotifyChange(IHubContext hubContext, SmsDTOWithOperation smsDTOWithOperation )
        {
            hubContext.Clients.All.notifyChangeToPage(smsDTOWithOperation);
            hubContext.Clients.All.notifyChangeToCharts();

            SendSMSHostContext db = new SendSMSHostContext();
            UpdateLog(db, smsDTOWithOperation);
        }

        public static void UpdateLog(SendSMSHostContext db, SmsDTOWithOperation smsDTOWithOperation)
        {
            db.Log.Add(new Log
            {
                SmsId = smsDTOWithOperation.SmsDTO.Id,
                Operation = smsDTOWithOperation.Operation,
                Timestamp = DateTime.Now,
                StatusName = smsDTOWithOperation.SmsDTO.StatusName
            });

            db.SaveChanges();
        }

        public ServerSentEventsHub()
        {
            db = new SendSMSHostContext();
        }
    }
}