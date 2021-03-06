namespace SendSMSHost.Migrations
{
    using SendSMSHost.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SendSMSHost.Models.SendSMSHostContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SendSMSHost.Models.SendSMSHostContext context)
        {
            context.Status.AddOrUpdate(x => x.Id,
                new Status { Id = 1, Name = "Created", DefaultColorHex = "#fdfdfe" },
                new Status { Id = 2, Name = "Queued", DefaultColorHex = "#bee5eb" },
                new Status { Id = 3, Name = "Pending", DefaultColorHex = "#ffeeba" },
                new Status { Id = 4, Name = "Sent", DefaultColorHex = "#c3e6cb" },
                new Status { Id = 0, Name = "Error", DefaultColorHex = "#f5c6cb" });

            context.SaveChanges();


            // Naam van Key aanpassen, nodig voor verbinding MS Access (er mag geen dbo. in staan)

            // Indices opvragen (https://stackoverflow.com/questions/7253943/entity-framework-code-first-find-primary-key)
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            ObjectSet<ImportSms> set = objectContext.CreateObjectSet<ImportSms>();
            IEnumerable<string> keyNames = set.EntitySet.ElementType
                                                        .KeyMembers
                                                        .Select(k => k.Name);

            if (keyNames.Any(x=> x == "PK_dbo.ImportSms"))
            {
                context.Database.ExecuteSqlCommand(
                    @"sp_rename '[dbo].[ImportSms].[PK_dbo.ImportSms]', 'PK_ImportSms'");
            }
        }
    }
}
