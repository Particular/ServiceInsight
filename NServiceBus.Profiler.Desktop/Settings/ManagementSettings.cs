using System.ComponentModel;
using System.Configuration;

namespace NServiceBus.Profiler.Desktop.Settings
{
    [SettingsProvider("Registry")]
    public class ManagementSettings
    {
        [DefaultValue("localhost")]
        public string Hostname { get; set; }

        [DefaultValue("")]
        [DisplayName("Virtual Directory")]
        public string VirtualDirectory { get; set; }

        [DefaultValue(8888)]
        [DisplayName("Port")]
        public int Port { get; set; }
    }
}