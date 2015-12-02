namespace Particular.ServiceInsight.Desktop.Settings
{
    using System.ComponentModel;
    using System.Configuration;

    [SettingsProvider("IsolatedStore")]
    public class SequenceDiagramSettings
    {
        [DefaultValue(true)]
        public bool ShowLegend { get; set; }
    }
}