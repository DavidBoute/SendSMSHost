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
        public int Status_Id { get; set; }
        public string Status_Name { get; set; }
        public string Contact_Id { get; set; }
        public string Contact_FirstName { get; set; }
        public string Contact_LastName { get; set; }
        public string Contact_Number { get; set; }
    }

    public class ContactDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
    }

    public class StatusDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
