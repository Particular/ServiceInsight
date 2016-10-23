namespace ServiceInsight.Framework.Logging
{
    using Serilog;

    class SerilogLogger : Pirac.ILogger
    {
        ILogger logger;

        public SerilogLogger(string type)
        {
            logger = Log.ForContext("SourceContext", type);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Info(string message)
        {
            logger.Information(message);
        }

        public void Warn(string message)
        {
            logger.Warning(message);
        }
    }
}