using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SendSMSHost.Models
{
    public class Sms
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }

        // Navigation properties
        public virtual Status Status { get; set; }
        public virtual Contact Contact { get; set; }

        public Sms CopyFromSmsDTO(SmsDTO smsDTO, ISendSMSHostContext db)
        {
            Message = smsDTO.Message;
            TimeStamp = DateTime.Parse(smsDTO.TimeStamp);
            Status = Status.FindStatusById(smsDTO.StatusId, db);

            Contact contact= Contact.FindOrCreate(
                new ContactDTO
                {
                    Id = smsDTO.ContactId,
                    FirstName = smsDTO.ContactFirstName,
                    LastName = smsDTO.ContactLastName,
                    Number = smsDTO.ContactNumber,
                    IsAnonymous = smsDTO.ContactFirstName == "" && smsDTO.ContactLastName == ""
                }, db);
            Contact = contact;

            return this;
        }

        public static Sms FindSmsById(Guid guid, ISendSMSHostContext db)
        {
                Sms sms = db.Sms
                    .Include("Contact")
                    .Include("Status")
                    .FirstOrDefault(x => x.Id == guid);
                return sms;
        }

        public static Sms FindSmsById(string guid, ISendSMSHostContext db)
        {
            return FindSmsById(Guid.Parse(guid),db);
        }

        public static Sms FindOrCreate(SmsDTO smsDTO, ISendSMSHostContext db)
        {
            Sms sms = FindSmsById(Guid.Parse(smsDTO.Id),db);
            if (sms == null)
            {
                sms = new Sms()
                {
                    Id = Guid.Parse(smsDTO.Id),
                    Message = smsDTO.Message,
                    TimeStamp = DateTime.Parse(smsDTO.TimeStamp),
                    Status = Status.FindStatusById(smsDTO.StatusId, db),
                    Contact = Contact.FindOrCreate(
                        new ContactDTO
                        {
                            Id = smsDTO.ContactId,
                            FirstName = smsDTO.ContactFirstName,
                            LastName = smsDTO.ContactLastName,
                            Number = smsDTO.ContactNumber,
                            IsAnonymous = smsDTO.ContactFirstName == "" && smsDTO.ContactLastName == ""
                        }, db)
                };
            }

            return sms;
        }

        public Sms()
        {

        }

        public Sms(SmsDTO smsDTO, ISendSMSHostContext db)
        {
            Id = Guid.Parse(smsDTO.Id);
            Message = smsDTO.Message;
            TimeStamp = DateTime.Parse(smsDTO.TimeStamp);
            Status = Status.FindStatusById(smsDTO.StatusId, db);
            Contact = new Contact
            {
                Id = Guid.Parse(smsDTO.ContactId),
                FirstName = smsDTO.ContactFirstName,
                LastName = smsDTO.ContactLastName,
                Number = smsDTO.ContactNumber,
                IsAnonymous = smsDTO.ContactFirstName == "" && smsDTO.ContactLastName == ""
            };
        }
    }
}