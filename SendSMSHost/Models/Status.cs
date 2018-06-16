using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SendSMSHost.Models
{
    public class Status
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DefaultColorHex { get; set; }

        // Navigation properties
        public virtual ICollection<Sms> Sms { get; set; }

        public static Status FindStatusById(int Id, ISendSMSHostContext db)
        {
                Status status = db.Status.First(x => x.Id == Id);
                return status;
        }
    }
}