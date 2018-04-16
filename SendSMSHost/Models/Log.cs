using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SendSMSHost.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        [Index]
        [MaxLength(36)]
        public string SmsId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }
        public string StatusName { get; set; }
    }
}