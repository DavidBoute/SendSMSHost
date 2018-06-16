using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SendSMSHost.Models
{
    public class SendSMSHostContext : DbContext, ISendSMSHostContext
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
        public DbSet<Log> Log { get; set; }

        internal void SaveChangesasync()
        {
            throw new NotImplementedException();
        }
    }

    public interface ISendSMSHostContext: IDisposable
    {
        DbSet<Contact> Contacts { get; set; }
        DbSet<Sms> Sms { get; set; }
        DbSet<Status> Status { get; set; }
        DbSet<ImportSms> ImportSms { get; set; }
        DbSet<Log> Log { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        Task<int> SaveChangesAsync();
    }
}
