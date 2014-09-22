namespace Particular.ServiceInsight.FunctionalTests.Framework
{
    using System;
    using System.IO;
    using Castle.Core.Logging;
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class NLogFactory : AbstractLoggerFactory
    {
        public NLogFactory()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget
            {
                FileName = Path.Combine(TestConfiguration.LogsFolder, "${shortdate}.txt"),
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}"
            };

            var rule1 = new LoggingRule("*", LogLevel.Debug, target);
            
            config.LoggingRules.Add(rule1);
            config.AddTarget("file", target);

            LogManager.Configuration = config;
        }

        public override ILogger Create(String name)
        {
            var log = LogManager.GetLogger(name);
            return new NLogLogger(log, this);
        }

        public override ILogger Create(String name, LoggerLevel level)
        {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }
    }
}