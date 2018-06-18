using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SendSMSHost.Models
{
    public class ImportSms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Message { get; set; }
        public string ContactNumber { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
    }

}