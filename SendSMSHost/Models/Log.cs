using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SendSMSHost.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        public string SmsId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }
        public string StatusName { get; set; }
    }
}