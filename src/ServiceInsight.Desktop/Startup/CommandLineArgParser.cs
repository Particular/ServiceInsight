namespace Particular.ServiceInsight.Desktop.Startup
{
    using System.Collections.Generic;
    using Models;
    using Serilog;

    public class CommandLineArgParser
    {
        static ILogger Logger = Log.ForContext<CommandLineArgParser>();// LogManager.GetLogger(typeof(CommandLineArgParser));

        const char UriSeparator = '?';
        const char TokenSeparator = '&';
        const char KeyValueSeparator = '=';

        EnvironmentWrapper environment;
        IList<string> unsupportedKeys;

        public CommandLineOptions ParsedOptions { get; private set; }

        public bool HasUnsupportedKeys
        {
            get { return unsupportedKeys.Count > 0; }
        }

        public CommandLineArgParser(EnvironmentWrapper environment)
        {
            this.environment = environment;
            unsupportedKeys = new List<string>();
            ParsedOptions = new CommandLineOptions();
        }

        public void Parse()
        {
            var args = environment.GetCommandLineArgs();

            Logger.Debug("Application invoked with following arguments: {args}", args);

            if (args.Length != 2) return;

            var uri = args[1].Split(UriSeparator);

            if (uri.Length == 0) return;

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

        void PopulateKeyValue(string key, string value)
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

                case "resetlayout":
                    ParsedOptions.SetResetLayout(bool.Parse(value));
                    break;

                default:
                    AddUnsupportedKey(key);
                    break;
            }
        }

        void AddUnsupportedKey(string key)
        {
            Logger.Warning("Key '{key}' is not supported.", key);
            unsupportedKeys.Add(key);
        }
    }
}