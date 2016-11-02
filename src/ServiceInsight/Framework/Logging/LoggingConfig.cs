namespace ServiceInsight.Framework.Logging
{
    using System;
    using System.IO;
    using System.Reactive.Linq;
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

            Func<LogEvent, bool> serviceControlLogFilter = le => Matching.FromSource<IServiceControl>()(le) || Matching.FromSource<IRxServiceControl>()(le);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()

                // Log to file
                .WriteTo.RollingFile(logPath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")

                // Log to trace
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Information()
                    .Filter.ByExcluding(serviceControlLogFilter)
                    .WriteTo.Trace(outputTemplate: "[{Level}] ({SourceContext}) {Message}{NewLine}{Exception}"))

                // Log to LogWindow
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Verbose()
                    .Filter.ByIncludingOnly(serviceControlLogFilter)
                    .WriteTo.Observers(logEvents => logEvents.Do(LogWindowViewModel.LogObserver).Subscribe()))
                .CreateLogger();
        }
    }
}