using System;
using Caliburn.Core.Logging;

namespace Particular.ServiceInsight.Desktop.Logging
{
    public class Log4NetLogger : ILog
    {
        private readonly log4net.ILog _logger;

        public Log4NetLogger()
        {
            _logger = log4net.LogManager.GetLogger("NServiceBus.Profiler");
        }

        public void Info(string message)
        {
            _logger.InfoFormat(message);
        }

        public void Warn(string message)
        {
            _logger.WarnFormat(message);
        }

        public void Error(Exception exception)
        {
            _logger.Error(string.Empty, exception);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }
    }
}