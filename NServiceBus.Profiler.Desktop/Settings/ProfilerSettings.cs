using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.PresentationFramework;

namespace NServiceBus.Profiler.Desktop.Settings
{
    public class ProfilerSettings : SettingBase
    {
        public ProfilerSettings()
        {
            RecentSearchEntries = new List<string>();
            RecentManagementApiEntries = new List<string>();
        }

        [DefaultValue(15)]
        [DisplayName("AutoRefresh Timer")]
        [Description("Auto refresh time in seconds")]
        public int AutoRefreshTimer { get; set; }

        [DefaultValue(false)]
        [DisplayName("Display System Messages")]
        [Description("Whether or not display system generated messages in the message list")]
        public bool DisplaySystemMessages { get; set; }

        [DefaultValue(false)]
        [DisplayName("Display Scheduled Messages")]
        [Description("Whether or not display scheduled messages in the message list")]
        public bool DisplayScheduledMessages { get; set; }

        [DisplayName("Recent Search Entries")]
        [Description("List of recent keywords used in search box")]
        public List<string> RecentSearchEntries { get; set; }

        [DisplayName("Recent Management API Entries")]
        [Description("List of recently connected service URLs for Management API")]
        public List<string> RecentManagementApiEntries { get; set; }

        [DisplayName("Management URL")]
        [Description("Last used Management API address")]
        public string LastUsedManagementApi { get; set; }
    }
}