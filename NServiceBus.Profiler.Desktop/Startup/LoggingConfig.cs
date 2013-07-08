using System;
using System.IO;
using Caliburn.Core.Logging;
using NServiceBus.Profiler.Desktop.Logging;

namespace NServiceBus.Profiler.Desktop.Startup
{
    public class LoggingConfig
    {
        public LoggingConfig()
        {
            SetupLog4net();
        }

        public ILog GetLogger()
        {
            return new CompositeLogger(new ILog[] { new Log4NetLogger(), new TraceLogger() });
        }

        public void SetupLog4net()
        {
            var logConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(logConfig));
        }
    }
}