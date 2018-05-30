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

        public int StatusId { get; set; }
        public virtual Status Status { get; set; }

        public Guid ContactId { get; set; }
        public virtual Contact Contact { get; set; }

        public Sms CopyFromSmsDTO(SmsDTO smsDTO)
        {
            Message = smsDTO.Message;
            TimeStamp = DateTime.Parse(smsDTO.TimeStamp);
            StatusId = smsDTO.StatusId;
            Status = Status.FindStatusById(smsDTO.StatusId);

            Contact contact= Contact.FindOrCreate(
                new ContactDTO
                {
                    Id = smsDTO.ContactId,
                    FirstName = smsDTO.ContactFirstName,
                    LastName = smsDTO.ContactLastName,
                    Number = smsDTO.ContactNumber,
                    IsAnonymous = smsDTO.ContactFirstName == "" && smsDTO.ContactLastName == ""
                });
            ContactId = contact.Id;
            Contact = contact;

            return this;
        }

        public static Sms FindSmsById(Guid guid)
        {
            using (SendSMSHostContext db = new SendSMSHostContext())
            {
                Sms sms = db.Sms
                    .Include("Contact")
                    .Include("Status")
                    .FirstOrDefault(x => x.Id == guid);
                return sms;
            }
        }

        public static Sms FindSmsById(string guid)
        {
            return FindSmsById(Guid.Parse(guid));
        }

        public static Sms FindOrCreate(SmsDTO smsDTO)
        {
            Sms sms = FindSmsById(Guid.Parse(smsDTO.Id));
            if (sms == null)
            {
                sms = new Sms()
                {
                    Id = Guid.Parse(smsDTO.Id),
                    Message = smsDTO.Message,
                    TimeStamp = DateTime.Parse(smsDTO.TimeStamp),
                    Status = Status.FindStatusById(smsDTO.StatusId),
                    Contact = Contact.FindOrCreate(
                        new ContactDTO
                        {
                            Id = smsDTO.ContactId,
                            FirstName = smsDTO.ContactFirstName,
                            LastName = smsDTO.ContactLastName,
                            Number = smsDTO.ContactNumber,
                            IsAnonymous = smsDTO.ContactFirstName == "" && smsDTO.ContactLastName == ""
                        })
                };
            }

            return sms;
        }

        public Sms()
        {

        }

        public Sms(SmsDTO smsDTO)
        {
            Id = Guid.Parse(smsDTO.Id);
            Message = smsDTO.Message;
            TimeStamp = DateTime.Parse(smsDTO.TimeStamp);
            Status = Status.FindStatusById(smsDTO.StatusId);
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