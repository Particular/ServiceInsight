using System.ComponentModel;
using System.Configuration;

namespace Particular.ServiceInsight.Desktop.Settings
{
    [SettingsProvider("IsolatedStore")]
    public class LicenseSettings
    {
        [DefaultValue(null)]
        public string HashedStartTrial { get; set; }
    }
}