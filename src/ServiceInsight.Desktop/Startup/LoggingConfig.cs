namespace Particular.ServiceInsight.Desktop.Startup
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
                .WriteTo.RollingFile(logPath)
                .WriteTo.Trace()
                .WriteTo.Logger(lc => lc
                    .MinimumLevel.Verbose()
                    .Filter.ByIncludingOnly(Matching.FromSource<DefaultServiceControl>())
                    .WriteTo.Observers(logEvents => logEvents.Do(LogWindowView.LogObserver).ObserveOnDispatcher().Subscribe()))
                .CreateLogger();
        }
    }
}