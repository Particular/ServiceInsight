using System;
using System.Diagnostics;
using Caliburn.Core.Logging;

namespace Particular.ServiceInsight.Desktop.Logging
{
    public class TraceLogger : ILog
    {
        public void Info(string message)
        {
            Trace.Write(message);
        }

        public void Warn(string message)
        {
            Trace.Write(message, "Warn");
        }

        public void Error(Exception exception)
        {
            Trace.Write(exception.GetBaseException().ToString(), "Error");
        }

        public void Error(string message, Exception exception)
        {
            Trace.Write(message + (exception != null ? exception.GetBaseException().ToString() : string.Empty), "Error");
        }
    }
}