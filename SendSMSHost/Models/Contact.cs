using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SendSMSHost.Models
{
    public class Contact
    {
        [Key]
        public Guid Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Number { get; set; }

        public bool IsAnonymous { get; set; }

        public virtual ICollection<Sms> Sms { get; set; }

        public static Contact GetContactByGuid(SendSMSHostContext context, Guid guid)
        {
            return context.Contacts.FirstOrDefault(x => x.Id == guid);
        }
    }
}