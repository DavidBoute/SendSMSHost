using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SendSMSHost.Models
{
    public class SendSMSHostContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public SendSMSHostContext() : base("name=SendSMSHostContext")
        {
        }

        public System.Data.Entity.DbSet<SendSMSHost.Models.Contact> Contacts { get; set; }

        public System.Data.Entity.DbSet<SendSMSHost.Models.Sms> Sms { get; set; }

        public System.Data.Entity.DbSet<SendSMSHost.Models.Status> Status { get; set; }

        public System.Data.Entity.DbSet<SendSMSHost.Models.ImportSms> ImportSms { get; set; }
    }
}
