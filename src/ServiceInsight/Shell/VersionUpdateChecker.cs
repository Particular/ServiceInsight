using RestSharp.Serialization.Json;
using ServiceInsight.ServiceControl;

namespace ServiceInsight.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Anotar.Serilog;
    using RestSharp;
    using RestSharp.Deserializers;
    using ServiceInsight.ExtensionMethods;

    public interface IVersionUpdateChecker
    {
        bool IsNewVersionAvailable();
    }

    public class VersionUpdateChecker : IVersionUpdateChecker
    {
        public bool IsNewVersionAvailable()
        {
            try
            {
                var latestVersionStr = CheckTheLatestVersion();
                var currentVersionStr = GetAppCurrentVersion();

                var latestVersion = new Version(latestVersionStr);
                var currentVersion = new Version(currentVersionStr);

                return latestVersion > currentVersion;
            }
            catch (Exception ex)
            {
                LogTo.Warning(ex, "Could not check for a new version.");
                return false;
            }
        }

        string CheckTheLatestVersion()
        {
            var client = new RestClient("https://s3.us-east-1.amazonaws.com");

            var request = new RestRequest("platformupdate.particular.net/serviceinsight.txt")
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };

            client.ClearHandlers();
            client.AddJsonDeserializer(new JsonDeserializer { DateFormat = RestRequestWithCache.CacheDateFormat });

            var rs = client.Execute<List<Release>>(request);

            return rs.Data.OrderByDescending(r => r.Published).First().Tag;
        }

        private static string GetAppCurrentVersion()
        {
            var assemblyInfo = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var versionParts = assemblyInfo.InformationalVersion.Split('+');
            var version = versionParts[0];
            var suffixIndex = version?.IndexOf("-");

            if (suffixIndex > 0)
            {
              return version.Substring(0, suffixIndex.Value);
            }

            return version;
        }

        class Release
        {
            public string Tag { get; set; }
            public DateTime Published { get; set; }
        }
    }
}
