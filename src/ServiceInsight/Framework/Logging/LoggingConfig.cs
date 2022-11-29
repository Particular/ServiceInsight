namespace ServiceInsight.Framework.Logging
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using Caliburn.Micro;
    using LogWindow;
    using MessageViewers.CustomMessageViewer;
    using Serilog;
    using Serilog.Core;
    using Serilog.Events;
    using Serilog.Filters;
    using ServiceControl;
    using ServiceInsight.MessageFlow;

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
                    .Filter.ByIncludingOnly(MatchingTypes(typeof(DefaultServiceControl), typeof(CustomMessageViewerResolver), typeof(MessageFlowViewModel)))
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

        static Func<LogEvent, bool> MatchingTypes(params Type[] type)
        {
            var scalars = type.Select(t => new ScalarValue(t.FullName));

            return e =>
            {
                return scalars.Any(s => e.Properties.TryGetValue(Constants.SourceContextPropertyName, out var propertyValue) && s.Equals(propertyValue));
            };
        }
    }
}
