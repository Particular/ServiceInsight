namespace ServiceInsight.Startup
{
    using System.Collections.Generic;
    using Anotar.Serilog;
    using Models;

    public class CommandLineArgParser
    {
        const char UriSeparator = '?';
        const char TokenSeparator = '&';
        const char KeyValueSeparator = '=';

        EnvironmentWrapper environment;
        IList<string> unsupportedKeys;

        public CommandLineOptions ParsedOptions { get; }

        public bool HasUnsupportedKeys => unsupportedKeys.Count > 0;

        public CommandLineArgParser(EnvironmentWrapper environment)
        {
            this.environment = environment;
            unsupportedKeys = new List<string>();
            ParsedOptions = new CommandLineOptions();
        }

        public void Parse()
        {
            var args = environment.GetCommandLineArgs();

            LogTo.Debug("Application invoked with following arguments: {args}", args);

            if (args.Length != 2)
            {
                return;
            }

            var uri = args[1].Split(UriSeparator);

            if (uri.Length == 0)
            {
                return;
            }

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
            LogTo.Warning("Key '{key}' is not supported.", key);
            unsupportedKeys.Add(key);
        }
    }
}