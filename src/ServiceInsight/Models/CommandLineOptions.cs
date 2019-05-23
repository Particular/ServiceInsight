namespace ServiceInsight.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using RestSharp.Extensions.MonoHttp;

    public class CommandLineOptions
    {
        public const string ApplicationScheme = "si://";

        private IEnumerable<string> AllowedSchemas
        {
            get
            {
                yield return "http";
                yield return "https";
            }
        }

        public Uri EndpointUri { get; private set; }

        public string SearchQuery { get; private set; }

        public string EndpointName { get; private set; }

        public int AutoRefreshRate { get; private set; }

        public bool ShouldAutoRefresh { get; private set; }

        public bool ResetLayout { get; private set; }

        public void SetEndpointUri(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var address = value;

            if (value.StartsWith(ApplicationScheme, StringComparison.OrdinalIgnoreCase))
            {
                address = value.Replace(ApplicationScheme,  "http://");
            }

            if (Uri.TryCreate(address, UriKind.Absolute, out var parsedEndpointUri) &&
                AllowedSchemas.Any(schema => schema.Equals(parsedEndpointUri.Scheme, StringComparison.InvariantCultureIgnoreCase)))
            {
                 EndpointUri = parsedEndpointUri;
            }
        }

        public void SetEndpointName(string value)
        {
            EndpointName = Decode(value);
        }

        public void SetSearchQuery(string value)
        {
            SearchQuery = Decode(value);
        }

        public void SetAutoRefresh(string value)
        {
            AutoRefreshRate = int.Parse(value);
            ShouldAutoRefresh = true;
        }

        public void SetResetLayout(bool value)
        {
            ResetLayout = value;
        }

        string Decode(string encodedString) => HttpUtility.UrlDecode(encodedString);
    }
}