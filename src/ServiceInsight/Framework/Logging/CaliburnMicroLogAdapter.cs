namespace Particular.ServiceInsight.Desktop.Framework.Logging
{
    using System;
    using Caliburn.Micro;
    using Serilog;

    public class CaliburnMicroLogAdapter : ILog
    {
        private readonly ILogger logger;

        public CaliburnMicroLogAdapter(ILogger logger)
        {
            this.logger = logger;
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