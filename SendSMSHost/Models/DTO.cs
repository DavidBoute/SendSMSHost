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
    }

    public class ContactDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public bool IsAnonymous { get; set; }
    }

    public class StatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SmsDTOWithOperation
    {
        public SmsDTO SmsDTO { get; set; }
        public string Operation { get; set; }
    }
}
