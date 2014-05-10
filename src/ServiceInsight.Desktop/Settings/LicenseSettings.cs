namespace Particular.ServiceInsight.Desktop.Settings
{
    using System.ComponentModel;
    using System.Configuration;

    [SettingsProvider("IsolatedStore")]
    public class LicenseSettings
    {
        [DefaultValue(null)]
        public string HashedStartTrial { get; set; }
    }
}