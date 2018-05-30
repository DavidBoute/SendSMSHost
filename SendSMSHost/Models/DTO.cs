using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SendSMSHost.Models
{
    public class SmsDTO
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string TimeStamp { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public string ContactId { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactNumber { get; set; }

        public SmsDTO()
        {

        }

        public SmsDTO(Sms sms)
        {
            Id = sms.Id.ToString();
            Message = sms.Message;
            TimeStamp = sms.TimeStamp.ToString();
            StatusId = sms.Status.Id;
            StatusName = sms.Status.Name;
            ContactId = sms.Contact.Id.ToString();
            ContactFirstName = sms.Contact.FirstName;
            ContactLastName = sms.Contact.LastName;
            ContactNumber = sms.Contact.Number;
    }
    }

    public class ContactDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public bool IsAnonymous { get; set; }

        public ContactDTO()
        {

        }

        public ContactDTO(Contact contact)
        {
            Id = contact.Id.ToString();
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Number = contact.Number;
            IsAnonymous = contact.IsAnonymous;
        }
    }

    public class StatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public StatusDTO()
        {

        }

        public StatusDTO(Status status)
        {
            Id = status.Id;
            Name = status.Name;
        }
    }

    public class SmsDTOWithOperation
    {
        public SmsDTO SmsDTO { get; set; }
        public string Operation { get; set; }
    }
}
