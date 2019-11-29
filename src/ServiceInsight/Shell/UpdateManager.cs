using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceInsight.Shell
{
    using System.Reflection;
    using Anotar.Serilog;
    using RestSharp;
    using RestSharp.Deserializers;
    using ServiceInsight.ExtensionMethods;

    public class UpdateManager
    {
        public bool NewUpdateIsAvailable()
        {
            try
            {
                var latestVersionStr = CheckTheLatestVersion();
                var currentVersionStr = GetAppCurrentVersion();

                var latestVersion = new Version(latestVersionStr);
                var currentVersion = new Version(currentVersionStr);

                if (latestVersion.CompareTo(currentVersion) > 0)
                {
                    return true;
                }
            }
            catch(Exception ex)
            {
                LogTo.Error(ex, "Check for a new version");

                //TODO: remove before merging to Master. Leaved it for tests
                return true;
            }

            return false;
        }

        string CheckTheLatestVersion()
        {
            var client = new RestClient("https://s3.us-east-1.amazonaws.com");

            var request = new RestRequest("platformupdate.particular.net/serviceinsight.txt")
            {
                OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; }
            };

            client.ClearHandlers();
            client.AddJsonDeserializer(new JsonDeserializer());

            var rs = client.Execute<List<Release>>(request);

            return rs.Data.OrderByDescending(r => r.Published).First().Tag;
        }

        private static string GetAppCurrentVersion()
        {
            var assemblyInfo = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var versionParts = assemblyInfo.InformationalVersion.Split('+');
            return versionParts[0];
        }

        class Release
        {
            public string Tag { get; set; }
            public DateTime Published { get; set; }
        }
    }
}
