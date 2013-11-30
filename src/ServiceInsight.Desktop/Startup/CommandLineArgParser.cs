using System;
using Autofac;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Startup
{
    public class CommandLineArgParser : ICommandLineArgParser
    {
        private const char UriSeparator = '?';
        private const char TokenSeparator = '&';
        private const char KeyValueSeparator = '=';

        private readonly IEnvironment _environment;

        public CommandLineOptions ParsedOptions { get; private set; }

        public CommandLineArgParser(IEnvironment environment)
        {
            _environment = environment;
            ParsedOptions = new CommandLineOptions();
        }

        public void Start()
        {
            var args = _environment.GetCommandLineArgs();

            if (args.Length != 2) return;

            var uri = args[1].Split(UriSeparator);

            if(uri.Length == 0) return;

            if (uri.Length > 0)
            {
                ParsedOptions.SetEndpointUri(uri[0]);
            }

            if (uri.Length > 1)
            {
                var parameters = uri[1];
                var tokens = parameters.Split(TokenSeparator);

                foreach (var token in tokens)
                {
                    var keyValue = token.Split(KeyValueSeparator);
                    if (keyValue.Length == 2)
                    {
                        PopulateKeyValue(keyValue[0], keyValue[1]);
                    }
                }
            }
        }

        private void PopulateKeyValue(string key, string value)
        {
            var parameter = key.ToLower();

            switch (parameter)
            {
                case "search":
                    ParsedOptions.SetSearchQuery(value);
                    break;
                case "endpointname":
                    ParsedOptions.SetEndpointName(value);
                    break;
                case "autorefresh":
                    ParsedOptions.SetAutoRefresh(value);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Key {0} is not supported.", key));
            }
        }
    }

    public interface ICommandLineArgParser : IStartable
    {
        CommandLineOptions ParsedOptions { get; }
    }
}