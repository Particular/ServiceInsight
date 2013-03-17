using System.ComponentModel;

namespace NServiceBus.Profiler.Common.Settings
{
    public class ReportingSettings
    {
        [DisplayName("Usage Data Collection Approved")]
        [DefaultValue(false)]
        public bool DataCollectionApproved { get; set; }
    }
}