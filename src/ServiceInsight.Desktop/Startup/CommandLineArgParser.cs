using System.Collections.Generic;
using log4net;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Startup
{
    public class CommandLineArgParser : ICommandLineArgParser
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ICommandLineArgParser));

        private const char UriSeparator = '?';
        private const char TokenSeparator = '&';
        private const char KeyValueSeparator = '=';

        private readonly IEnvironment _environment;
        private readonly IList<string> _unsupportedKeys;
        
        public CommandLineOptions ParsedOptions { get; private set; }

        public bool HasUnsupportedKeys
        {
            get { return _unsupportedKeys.Count > 0; }
        }

        public CommandLineArgParser(IEnvironment environment)
        {
            _environment = environment;
            _unsupportedKeys = new List<string>();
            ParsedOptions = new CommandLineOptions();
        }

        public void Parse()
        {
            var args = _environment.GetCommandLineArgs();
            
            Logger.DebugFormat("Application invoked with following arguments: {0}", string.Join(" ", args));

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
                    AddUnsupportedKey(key);
                    break;
            }
        }

        private void AddUnsupportedKey(string key)
        {
            Logger.WarnFormat("Key '{0}' is not supported.", key);
            _unsupportedKeys.Add(key);
        }
    }

    public interface ICommandLineArgParser
    {
        CommandLineOptions ParsedOptions { get; }
        bool HasUnsupportedKeys { get; }
    }
}