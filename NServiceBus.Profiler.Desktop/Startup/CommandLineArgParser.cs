using System;
using log4net;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Startup
{
    public class CommandLineArgParser : ICommandLineArgParser
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (ICommandLineArgParser));
        private const char TokenSeparator = '&';
        private const char KeyValueSeparator = '=';

        public CommandLineOptions GetCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();
            var result = new CommandLineOptions();

            if (args.Length != 2) return result;

            var parameters = args[1].ToLower();
            var tokens = parameters.Split(TokenSeparator);

            foreach (var token in tokens)
            {
                var keyValue = token.Split(KeyValueSeparator);
                if (keyValue.Length == 1)
                {
                    result.SetEndpointUri(token);
                }
            }

            return result;
        }
    }
    
    public interface ICommandLineArgParser
    {
        CommandLineOptions GetCommandLineArgs();
    }
}