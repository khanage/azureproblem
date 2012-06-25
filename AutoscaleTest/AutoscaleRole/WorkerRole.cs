using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.Autoscaling;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AutoscaleRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private Autoscaler autoscaler;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("Csn.ImageServer.AutoScaler entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            CloudStorageAccount.SetConfigurationSettingPublisher(
              (configName, configSetter) => configSetter(RoleEnvironment.GetConfigurationSettingValue(configName)));

            var dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();
            dmc.Logs.BufferQuotaInMB = 4;
            dmc.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;
            DiagnosticMonitor.Start(
              "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", dmc);

            autoscaler = EnterpriseLibraryContainer.Current.GetInstance<Autoscaler>();
            autoscaler.Start();

            return base.OnStart();
        }

        public override void OnStop()
        {
            autoscaler.Stop();
            base.OnStop();
        }
    }
}
