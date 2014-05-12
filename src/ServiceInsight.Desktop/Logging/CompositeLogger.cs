namespace Particular.ServiceInsight.Desktop.Logging
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Core.Logging;
    using ExtensionMethods;

    public class CompositeLogger : ILog
    {
        readonly IList<ILog> loggers;

        public CompositeLogger(IList<ILog> loggers)
        {
            this.loggers = loggers;
        }

        public void Info(string message)
        {
            loggers.ForEach(x => x.Info(message));
        }

        public void Warn(string message)
        {
            loggers.ForEach(x => x.Warn(message));
        }

        public void Error(Exception exception)
        {
            loggers.ForEach(x => x.Error(exception));
        }

        public void Error(string message, Exception exception)
        {
            loggers.ForEach(x => x.Error(message, exception));
        }
    }
}