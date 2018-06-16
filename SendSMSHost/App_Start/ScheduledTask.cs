using FluentScheduler;
using Microsoft.AspNet.SignalR;
using SendSMSHost.Helpers;
using SendSMSHost.Models;
using SendSMSHost.SignalR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web.Hosting;

namespace SendSMSHost
{
    // https://github.com/fluentscheduler/FluentScheduler
    public class ScheduledTask
    {
        public class TaskRegistry : Registry
        {
            public TaskRegistry()
            {
                Schedule<EnqueueSmsToSend>().ToRunEvery(1).Seconds();
                Schedule<ImportSms>().ToRunEvery(10).Seconds();
            }
        }

        static bool IsBusyImporting { get; set; }

        public class EnqueueSmsToSend : IJob, IRegisteredObject
        {
            const int BATCHSIZE = 5;

            private readonly object _lock = new object();
            private bool _shuttingDown;

            public EnqueueSmsToSend()
            {
                // Register this job with the hosting environment.
                // Allows for a more graceful stop of the job, in the case of IIS shutting down.
                HostingEnvironment.RegisterObject(this);
            }

            public void Execute()
            {
                try
                {
                    lock (_lock)
                    {
                        if (_shuttingDown)
                            return;

                        if (IsBusyImporting)
                            return;

                        SmsProcessingHelper smsHelper = new SmsProcessingHelper(GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>());
                        smsHelper.Enqueue(BATCHSIZE);

                    }
                }
                finally
                {
                    // Always unregister the job when done.
                    HostingEnvironment.UnregisterObject(this);
                }
            }

            public void Stop(bool immediate)
            {
                // Locking here will wait for the lock in Execute to be released until this code can continue.
                lock (_lock)
                {
                    _shuttingDown = true;
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }

        public class ImportSms : IJob, IRegisteredObject
        {
            private readonly object _lock = new object();
            private bool _shuttingDown;

            public ImportSms()
            {
                // Register this job with the hosting environment.
                // Allows for a more graceful stop of the job, in the case of IIS shutting down.
                HostingEnvironment.RegisterObject(this);
            }

            public void Execute()
            {
                try
                {
                    lock (_lock)
                    {
                        if (_shuttingDown)
                            return;

                        if (IsBusyImporting)
                            return;

                        IsBusyImporting = true;

                        SmsProcessingHelper smsHelper = new SmsProcessingHelper(GlobalHost.ConnectionManager.GetHubContext<ServerSentEventsHub>());
                        smsHelper.Import();
                        
                        IsBusyImporting = false;
                        Debug.WriteLine($"[{DateTime.Now}] Importing finished");
                    }
                }
                finally
                {
                    // Always unregister the job when done.
                    HostingEnvironment.UnregisterObject(this);
                }
            }

            public void Stop(bool immediate)
            {
                // Locking here will wait for the lock in Execute to be released until this code can continue.
                lock (_lock)
                {
                    _shuttingDown = true;
                }

                HostingEnvironment.UnregisterObject(this);
            }
        }
    }
}