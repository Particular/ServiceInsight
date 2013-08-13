using System.ComponentModel;

namespace Particular.ServiceInsight.Desktop.Settings
{
    public class ReportingSettings
    {
        [DisplayName("Usage Data Collection Approved")]
        [DefaultValue(false)]
        public bool DataCollectionApproved { get; set; }
    }
}