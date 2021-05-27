namespace ServiceInsight.Startup
{
    using System;
    using System.IO.Pipes;
    using System.Collections.Generic;
    using System.IO;
    using Anotar.Serilog;
    using ServiceInsight.Models;

    public class CommandLineArgParser
    {
        const char UriSeparator = '?';
        const char TokenSeparator = '&';
        const char KeyValueSeparator = '=';

        EnvironmentWrapper environment;
        IList<string> unsupportedKeys;

        public CommandLineOptions ParsedOptions { get; private set; }

        public bool HasUnsupportedKeys => unsupportedKeys.Count > 0;

        public CommandLineArgParser() : this(new EnvironmentWrapper())
        {
        }

        public CommandLineArgParser(EnvironmentWrapper environment)
        {
            this.environment = environment;
            unsupportedKeys = new List<string>();
            ParsedOptions = new CommandLineOptions();

            Parse();
        }

        public void SendToOtherInstance()
        {
            var args = environment.GetCommandLineArgs();

            if (args.Length != 2)
            {
                return;
            }

            try
            {
                using (var pipe = new NamedPipeClientStream(".", $"ServiceInsight-{Environment.UserName}", PipeDirection.Out))
                using (var writer = new StreamWriter(pipe))
                {
                    pipe.Connect(1000);
                    writer.Write(args[1]); //Second element contains all the args
                }
            }
            catch (Exception ex)
            {
                LogTo.Information(ex, "Could not send command line arguments another ServiceInsight instance.");
            }
        }

        public void Parse(string[] passedArgs = null)
        {
            var args = passedArgs ?? environment.GetCommandLineArgs();

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

            // should be done last
            if (uri.Length > 0)
            {
                ParsedOptions.SetEndpointUri(uri[0]);
            }
        }

        void PopulateKeyValue(string key, string value)
        {
            var parameter = key.ToLower();

            switch (parameter)
            {
                case "securedconnection":
                    ParsedOptions.SetSecuredConnection(bool.Parse(value));
                    break;

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
                case "silentstartup":
                    ParsedOptions.SetSilentStartup(bool.Parse(value));
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