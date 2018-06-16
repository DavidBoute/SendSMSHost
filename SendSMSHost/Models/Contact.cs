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

        // Navigation properties
        public virtual ICollection<Sms> Sms { get; set; }

        public static Contact FindContactById(Guid guid,ISendSMSHostContext db)
        {
                Contact contact = db.Contacts.FirstOrDefault(x => x.Id == guid);
                return contact;
        }

        public static Contact FindOrCreate(ContactDTO contactDTO, ISendSMSHostContext db)
        {
            Contact contact = FindContactById(Guid.Parse(contactDTO.Id),db);

            return contact ?? new Contact(contactDTO);
        }

        public Contact()
        {

        }

        public Contact(ContactDTO contactDTO)
        {
            Id = Guid.Parse(contactDTO.Id);
            FirstName = contactDTO.FirstName;
            LastName = contactDTO.LastName;
            Number = contactDTO.Number;
            IsAnonymous = contactDTO.IsAnonymous;
        }

    }
}