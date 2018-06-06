namespace ServiceInsight.Shell
{
    using System;
    using System.Linq;
    using System.Reflection;
    using ServiceInsight.ExtensionMethods;

    public interface IApplicationVersionService
    {
        string GetVersion();

        string GetCommitHash();
    }

    public class ApplicationVersionService : IApplicationVersionService
    {
        public string GetVersion()
        {
            var versionParts = GetVerionParts();
            var appVersion = versionParts[0];

            return appVersion;
        }

        public string GetCommitHash()
        {
            var metadata = GetVerionParts().Last();
            var parts = metadata.Split('.');
            var shaIndex = parts.IndexOf("Sha", StringComparer.InvariantCultureIgnoreCase);
            if (shaIndex != -1 && parts.Length > shaIndex + 1)
            {
                var shaValue = parts[shaIndex + 1];
                var shortCommitHash = shaValue.Substring(0, 7);

                return shortCommitHash;
            }

            return null;
        }

        private static string[] GetVerionParts()
        {
            var assemblyInfo = typeof(App).Assembly.GetAttribute<AssemblyInformationalVersionAttribute>();
            var version = assemblyInfo != null ? assemblyInfo.InformationalVersion : "Unknown Version";
            var versionParts = version.Split('+');
            return versionParts;
        }
    }
}