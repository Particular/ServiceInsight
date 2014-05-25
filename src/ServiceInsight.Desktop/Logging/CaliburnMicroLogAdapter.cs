namespace Particular.ServiceInsight.Desktop.Logging
{
    using System;
    using Caliburn.Micro;
    using Serilog;

    public class CaliburnMicroLogAdapter : ILog
    {
        ILogger logger;

        public CaliburnMicroLogAdapter()
        {
            logger = Log.ForContext<CaliburnMicroLogAdapter>();
        }

        public void Info(string message)
        {
            logger.Information(message);
        }

        public void Info(string format, params object[] args)
        {
            logger.Information(format, args);
        }

        public void Warn(string message)
        {
            logger.Warning(message);
        }

        public void Warn(string format, params object[] args)
        {
            logger.Warning(format, args);
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