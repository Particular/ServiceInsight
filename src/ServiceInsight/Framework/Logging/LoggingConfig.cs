namespace ServiceInsight.Framework.Logging
{
    using System;
    using System.IO;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using LogWindow;
    using Serilog;
    using Serilog.Events;
    using Serilog.Filters;
    using ServiceControl;

    public static class LoggingConfig
    {
        public static void SetupLogging()
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Particular", "ServiceInsight", "log-{Date}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Filter.ByExcluding(le => Matching.FromSource(typeof(Screen).FullName)(le) && le.Level <= LogEventLevel.Information)  // Turn off some of Caliburn.Micro's chattiness
                .Filter.ByExcluding(le => Matching.FromSource(typeof(Caliburn.Micro.Action).FullName)(le) && le.Level <= LogEventLevel.Information)
                .Filter.ByExcluding(le => Matching.FromSource(typeof(ActionMessage).FullName)(le) && le.Level <= LogEventLevel.Information)
                .Filter.ByExcluding(le => Matching.FromSource(typeof(ViewModelBinder).FullName)(le) && le.Level <= LogEventLevel.Information)

                .WriteTo.RollingFile(logPath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
                .WriteTo.Trace(outputTemplate: "[{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Verbose()
                    .Filter.ByIncludingOnly(Matching.FromSource<DefaultServiceControl>())
                    .WriteTo.Observers(logEvents => logEvents
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Do(LogWindowViewModel.LogObserver)
                        .Subscribe()))
                .CreateLogger();
        }

        public static void SetupCaliburnMicroLogging()
        {
            LogManager.GetLog = type => new CaliburnMicroLogAdapter(Log.ForContext(type));
        }
    }
}
