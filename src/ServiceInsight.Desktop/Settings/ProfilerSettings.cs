namespace Particular.ServiceInsight.Desktop.Settings
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class ProfilerSettings : SettingBase
    {
        int autoRefresh;

        public ProfilerSettings()
        {
            RecentSearchEntries = new ObservableCollection<string>();
            RecentServiceControlEntries = new ObservableCollection<string>();
        }

        [DefaultValue(15)]
        [DisplayName("AutoRefresh Timer")]
        [Description("Auto refresh time in seconds")]
        public int AutoRefreshTimer
        {
            get { return Math.Max(1, autoRefresh); }
            set { autoRefresh = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Display System Messages")]
        [Description("Whether or not display system generated messages in the message list")]
        public bool DisplaySystemMessages { get; set; }

        [DefaultValue(false)]
        [DisplayName("Display Scheduled Messages")]
        [Description("Whether or not display scheduled messages in the message list")]
        public bool DisplayScheduledMessages { get; set; }

        [DefaultValue(false)]
        [DisplayName("Display Endpoint Information")]
        [Description("Whether or not display Endpoint information in diagrams")]
        public bool ShowEndpoints { get; set; }

        [DisplayName("Recent Search Entries")]
        [Description("List of recent keywords used in search box")]
        public ObservableCollection<string> RecentSearchEntries { get; set; }

        [DisplayName("Recent ServiceControl Entries")]
        [Description("List of recently connected service URLs for ServiceControl")]
        public ObservableCollection<string> RecentServiceControlEntries { get; set; }

        [DisplayName("ServiceControl URL")]
        [Description("Last used ServiceControl address")]
        public string LastUsedServiceControl { get; set; }
    }
}