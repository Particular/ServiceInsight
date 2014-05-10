namespace Particular.ServiceInsight.Desktop.Startup
{
    using System;
    using System.IO;

    public static class LoggingConfig
    {
        public const string LogPattern = "%date - [%-5level] - %logger{1} - %message%newline";

        public static void SetupLog4net()
        {
            var logConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfig));
        }
    }
}