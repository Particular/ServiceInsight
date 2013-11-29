using System;
using System.Text.RegularExpressions;
using RestSharp.Contrib;

namespace NServiceBus.Profiler.Desktop.Models
{
    public class CommandLineOptions
    {
        public const string ApplicationScheme = "si://";
        public const string UriRegexPattern = @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&amp;%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&amp;%\$#\=~_\-]+))*$";
        
        public Uri EndpointUri { get; private set; }
        public string SearchQuery { get; private set; }
        public string EndpointName { get; private set; }
        public int AutoRefreshRate { get; private set; }

        public void SetEndpointUri(string value)
        {
            if(string.IsNullOrWhiteSpace(value)) return;
            var address = value;

            if (value.StartsWith(ApplicationScheme, StringComparison.OrdinalIgnoreCase))
            {
                address = value.Remove(0, ApplicationScheme.Length);
            }

            var regex = new Regex(UriRegexPattern);
            if (regex.IsMatch(address))
            {
                EndpointUri = new Uri(address);
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
        }

        private string Decode(string encodedString)
        {
            return HttpUtility.UrlDecode(encodedString);
        }
    }
}