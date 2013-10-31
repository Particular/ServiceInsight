using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace NServiceBus.Profiler.Desktop.Settings
{
    public class ProfilerSettings : SettingBase
    {
        private int _autoRefresh;

        public ProfilerSettings()
        {
            RecentSearchEntries = new ObservableCollection<string>();
            RecentManagementApiEntries = new ObservableCollection<string>();
        }

        [DefaultValue(15)]
        [DisplayName("AutoRefresh Timer")]
        [Description("Auto refresh time in seconds")]
        public int AutoRefreshTimer
        {
            get { return Math.Max(10, _autoRefresh); }
            set { _autoRefresh = value; }
        }

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
        public ObservableCollection<string> RecentSearchEntries { get; set; }

        [DisplayName("Recent Service Control Entries")]
        [Description("List of recently connected service URLs for Service Control")]
        public ObservableCollection<string> RecentManagementApiEntries { get; set; }

        [DisplayName("Service Control URL")]
        [Description("Last used Service Control address")]
        public string LastUsedManagementApi { get; set; }
    }
}