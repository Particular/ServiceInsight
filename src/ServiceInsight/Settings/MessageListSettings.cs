namespace ServiceInsight.Settings
{
    using System.ComponentModel;
    using System.Configuration;

    [SettingsProvider("IsolatedStore")]
    public class MessageListSettings
    {
        [DefaultValue(null)]
        public string GridLayout { get; set; }
    }
}