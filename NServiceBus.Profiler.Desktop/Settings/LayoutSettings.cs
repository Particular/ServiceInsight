using System.ComponentModel;
using System.Configuration;

namespace NServiceBus.Profiler.Desktop.Settings
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

        [DefaultValue(false)]
        public bool ResetLayout { get; set; }
    }
}