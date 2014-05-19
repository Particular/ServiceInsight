namespace Particular.ServiceInsight.Desktop.Logging
{
    using System;
    using Caliburn.Micro;

    public class Log4NetLogger : ILog
    {
        log4net.ILog logger;

        public Log4NetLogger()
        {
            logger = log4net.LogManager.GetLogger("NServiceBus.Profiler");
        }

        public void Info(string message)
        {
            logger.InfoFormat(message);
        }

        public void Info(string format, params object[] args)
        {
            logger.InfoFormat(format, args);
        }

        public void Warn(string message)
        {
            logger.WarnFormat(message);
        }

        public void Warn(string format, params object[] args)
        {
            logger.WarnFormat(format, args);
        }

        public void Error(Exception exception)
        {
            logger.Error(string.Empty, exception);
        }

        public void Error(string message, Exception exception)
        {
            logger.Error(message, exception);
        }
    }
}