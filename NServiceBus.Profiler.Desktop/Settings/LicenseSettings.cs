using System.ComponentModel;
using System.Configuration;

namespace NServiceBus.Profiler.Desktop.Settings
{
    [SettingsProvider("IsolatedStore")]
    public class LicenseSettings
    {
        [DefaultValue(null)]
        public string HashedStartTrial { get; set; }
    }
}