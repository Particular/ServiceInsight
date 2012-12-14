using System;
using System.Collections.Generic;
using Caliburn.Core.Logging;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.Logging
{
    public class CompositeLogger : ILog
    {
        private readonly IList<ILog> _loggers;

        public CompositeLogger(IList<ILog> loggers)
        {
            _loggers = loggers;
        }

        public void Info(string message)
        {
            _loggers.ForEach(x => x.Info(message));
        }

        public void Warn(string message)
        {
            _loggers.ForEach(x => x.Warn(message));
        }

        public void Error(Exception exception)
        {
            _loggers.ForEach(x => x.Error(exception));
        }

        public void Error(string message, Exception exception)
        {
            _loggers.ForEach(x => x.Error(message, exception));
        }
    }
}