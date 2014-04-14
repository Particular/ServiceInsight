using System.ComponentModel;

namespace NServiceBus.Profiler.Desktop.Settings
{
    public class ReportingSettings
    {
        [DisplayName("Usage Data Collection Approved")]
        [DefaultValue(false)]
        public bool DataCollectionApproved { get; set; }
    }
}