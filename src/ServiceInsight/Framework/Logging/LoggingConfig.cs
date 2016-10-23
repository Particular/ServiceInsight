namespace ServiceInsight.Framework.Logging
{
    using System;
    using System.IO;
    using System.Reactive.Linq;
    using LogWindow;
    using Serilog;
    using Serilog.Filters;
    using ServiceControl;

    public static class LoggingConfig
    {
        public static void SetupLogging()
        {
            var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Particular", "ServiceInsight", "log-{Date}.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.RollingFile(logPath, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
                .WriteTo.Trace(outputTemplate: "[{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Verbose()
                    .Filter.ByIncludingOnly(le => Matching.FromSource<IServiceControl>()(le) || Matching.FromSource<IRxServiceControl>()(le))
                    .WriteTo.Observers(logEvents => logEvents.Do(LogWindowViewModel.LogObserver).Subscribe()))
                .CreateLogger();
        }
    }
}