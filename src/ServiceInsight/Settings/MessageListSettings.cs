namespace ServiceInsight.Settings
{
    using System.ComponentModel;
    using System.Configuration;

    [SettingsProvider("AppDataStore")]
    public class MessageListSettings
    {
        [DefaultValue(null)]
        public string GridLayout { get; set; }
    }
}