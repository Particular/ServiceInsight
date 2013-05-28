using System.ComponentModel;
using System.Configuration;

namespace NServiceBus.Profiler.Common.Settings
{
    [SettingsProvider("IsolatedStore")]
    public class LicenseSettings
    {
        [DefaultValue(null)]
        public string HashedStartTrial { get; set; }
    }
}