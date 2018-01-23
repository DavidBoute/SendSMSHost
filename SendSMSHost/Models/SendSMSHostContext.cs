﻿using System.Data.Entity;

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

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Sms> Sms { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<ImportSms> ImportSms { get; set; }
    }
}
