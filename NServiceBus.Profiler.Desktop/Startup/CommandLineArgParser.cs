using System;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Startup
{
    public class CommandLineArgParser : ICommandLineArgParser
    {
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
                if (keyValue.Length == 2)
                {
                    PopulateKeyValue(result, keyValue[0], keyValue[1]);
                }
                else
                {
                    result.SetEndpointUri(token);
                }
            }

            return result;
        }

        private void PopulateKeyValue(CommandLineOptions options, string key, string value)
        {
            switch (key)
            {
                case "search":
                    options.SetSearchQuery(value);
                    break;
                case "endpointname":
                    options.SetEndpointName(value);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Key {0} is not supported.", key));
            }
        }
    }

    public interface ICommandLineArgParser
    {
        CommandLineOptions GetCommandLineArgs();
    }
}