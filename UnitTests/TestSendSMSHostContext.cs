using SendSMSHost.Models;
using System.Data.Common;
using System.Data.Entity;

namespace UnitTests
{
    public class TestSendSMSHostContext : DbContext, ISendSMSHostContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public TestSendSMSHostContext(DbConnection connection) : base(connection, true)
        {
            this.Database.CreateIfNotExists();
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Sms> Sms { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<ImportSms> ImportSms { get; set; }
        public DbSet<Log> Log { get; set; }
    }
}
