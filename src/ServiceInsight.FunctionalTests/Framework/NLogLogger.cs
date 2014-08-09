namespace Particular.ServiceInsight.FunctionalTests
{
    using System;
    using Castle.Core.Logging;
    using NLog;

    public class NLogLogger : ILogger
    {
        public NLogLogger(Logger logger, NLogFactory factory)
        {
            Logger = logger;
            Factory = factory;
        }

        public bool IsDebugEnabled
        {
            get { return Logger.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return Logger.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return Logger.IsFatalEnabled; }
        }
        
        public bool IsInfoEnabled
        {
            get { return Logger.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return Logger.IsWarnEnabled; }
        }

        protected internal NLogFactory Factory { get; set; }

        protected internal Logger Logger { get; set; }

        public override string ToString()
        {
            return Logger.ToString();
        }

        public virtual ILogger CreateChildLogger(String name)
        {
            return Factory.Create(Logger.Name + "." + name);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Debug(Func<string> messageFactory)
        {
            if (IsDebugEnabled == false)
            {
                return;
            }
            Log(LogLevel.Debug, messageFactory());
        }

        public void Debug(string message, Exception exception)
        {
            Log(LogLevel.Debug, message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        public void DebugFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Debug, exception, format, args);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Debug, formatProvider, format, args);
        }

        public void DebugFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Debug, exception, formatProvider, format, args);
        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Error(Func<string> messageFactory)
        {
            if (IsErrorEnabled == false)
            {
                return;
            }
            Log(LogLevel.Error, messageFactory());
        }

        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        public void ErrorFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Error, exception, format, args);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Error, formatProvider, format, args);
        }

        public void ErrorFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Error, exception, formatProvider, format, args);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public void Fatal(Func<string> messageFactory)
        {
            if (IsFatalEnabled == false)
            {
                return;
            }
            Log(LogLevel.Fatal, messageFactory());
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            Log(LogLevel.Fatal, format, args);
        }

        public void FatalFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Fatal, exception, format, args);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Fatal, formatProvider, format, args);
        }

        public void FatalFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Fatal, exception, formatProvider, format, args);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Info(Func<string> messageFactory)
        {
            if (IsInfoEnabled == false)
            {
                return;
            }
            Log(LogLevel.Info, messageFactory());
        }

        public void Info(string message, Exception exception)
        {
            Log(LogLevel.Info, message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        public void InfoFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Info, exception, format, args);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Info, formatProvider, format, args);
        }

        public void InfoFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Info, exception, formatProvider, format, args);
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void Warn(Func<string> messageFactory)
        {
            if (IsWarnEnabled == false)
            {
                return;
            }
            Log(LogLevel.Warn, messageFactory());
        }

        public void Warn(string message, Exception exception)
        {
            Log(LogLevel.Warn, message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        public void WarnFormat(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Warn, exception, format, args);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Warn, formatProvider, format, args);
        }

        public void WarnFormat(Exception exception, IFormatProvider formatProvider, string format, params object[] args)
        {
            Log(LogLevel.Warn, exception, formatProvider, format, args);
        }

        private void Log(LogLevel logLevel, string message)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, message));
        }

        private void Log(LogLevel logLevel, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                Parameters = args
            });
        }

        private void Log(LogLevel logLevel, string message, Exception exception)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, message)
            {
                Exception = exception
            });
        }

        private void Log(LogLevel logLevel, Exception exception, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                Exception = exception,
                Parameters = args
            });
        }

        private void Log(LogLevel logLevel, IFormatProvider formatProvider, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                FormatProvider = formatProvider,
                Parameters = args
            });
        }

        private void Log(LogLevel logLevel, Exception exceptoin, IFormatProvider formatProvider, string format, object[] args)
        {
            Logger.Log(typeof(NLogLogger), new LogEventInfo(logLevel, Logger.Name, format)
            {
                Exception = exceptoin,
                FormatProvider = formatProvider,
                Parameters = args
            });
        }
    }
}