namespace Particular.ServiceInsight.Desktop.Framework.Logging
{
    using System;
    using Anotar.Serilog;
    using Caliburn.Micro;

    public class CaliburnMicroLogAdapter : ILog
    {
        public void Info(string message)
        {
            LogTo.Information(message);
        }

        public void Info(string format, params object[] args)
        {
            LogTo.Information(format, args);
        }

        public void Warn(string message)
        {
            LogTo.Warning(message);
        }

        public void Warn(string format, params object[] args)
        {
            LogTo.Warning(format, args);
        }

        public void Error(Exception exception)
        {
            LogTo.Error(string.Empty, exception);
        }

        public void Error(string message, Exception exception)
        {
            LogTo.Error(message, exception);
        }
    }
}