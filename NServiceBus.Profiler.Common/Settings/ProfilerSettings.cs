using System.Collections.Generic;

namespace NServiceBus.Profiler.Common.Settings
{
    public class ProfilerSettings
    {
        public List<string> RecentSearchEntries { get; set; }

        public List<string> RecentManagementApiEntries { get; set; }

        public string LastUsedManagementApi { get; set; }
    }
}