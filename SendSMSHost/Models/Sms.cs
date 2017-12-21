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

    }
}