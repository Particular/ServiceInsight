using System.ComponentModel;
using System.Configuration;

namespace NServiceBus.Profiler.Common.Settings
{
    [SettingsProvider("IsolatedStore")]
    public class ShellLayoutSettings
    {
        [DefaultValue(null)]
        public string DockLayout { get; set; }

        [DefaultValue(null)]
        public string MenuLayout { get; set; }

        [DefaultValue(null)]
        public string LayoutVersion { get; set; }
    }
}