using System.Collections.Generic;

namespace NServiceBus.Profiler.Desktop.Settings
{
    public class ProfilerSettings
    {
        public ProfilerSettings()
        {
            RecentSearchEntries = new List<string>();
            RecentManagementApiEntries = new List<string>();
        }

        public List<string> RecentSearchEntries { get; set; }

        public List<string> RecentManagementApiEntries { get; set; }

        public string LastUsedManagementApi { get; set; }
    }
}