namespace ServiceInsight.Settings
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Caliburn.Micro;

    // Properties without a DisplayNameAttribute aren't automatically added to the options dialog.
    public class ProfilerSettings : PropertyChangedBase
    {
        int autoRefresh, cacheSize;

        public ProfilerSettings()
        {
            RecentSearchEntries = new ObservableCollection<string>();
            RecentServiceControlEntries = new ObservableCollection<string>();
        }

        [DefaultValue(20)]
        [DisplayName("In-Memory Cache Size")]
        [Description("Sets the maximum size to use for caching data in-memory in MB")]
        public int CacheSize
        {
            get { return Math.Max(5, cacheSize); }
            set { cacheSize = value; }
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

        //[DisplayName("Recent Search Entries")]
        //[Description("List of recent keywords used in search box")]
        public ObservableCollection<string> RecentSearchEntries { get; set; }

        [DisplayName("Recent ServiceControl Entries")]
        [Description("List of recently connected service URLs for ServiceControl")]
        public ObservableCollection<string> RecentServiceControlEntries { get; set; }

        [DisplayName("ServiceControl URL")]
        [Description("Last used ServiceControl address")]
        public string LastUsedServiceControl { get; set; }

        public string DefaultLastUsedServiceControl { get; set; }

        [DisplayName("Usage Data Collection Approved")]
        [DefaultValue(false)]
        public bool DataCollectionApproved { get; set; }
    }
}