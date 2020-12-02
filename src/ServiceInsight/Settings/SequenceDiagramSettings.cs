namespace ServiceInsight.Settings
{
    using System.ComponentModel;
    using System.Configuration;

    [SettingsProvider("AppDataStore")]
    public class SequenceDiagramSettings
    {
        [DefaultValue(true)]
        public bool ShowLegend { get; set; }
    }
}