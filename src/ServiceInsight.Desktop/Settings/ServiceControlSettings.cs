using System.ComponentModel;
using System.Configuration;

namespace NServiceBus.Profiler.Desktop.Settings
{
    [SettingsProvider("Registry")]
    public class ServiceControlSettings
    {
        [DefaultValue(33333)]
        public int Port { get; set; }
        public string TransportType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}