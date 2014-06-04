namespace Particular.ServiceInsight.Desktop.Settings
{
    using System.ComponentModel;

    public class ReportingSettings
    {
        [DisplayName("Usage Data Collection Approved")]
        [DefaultValue(false)]
        public bool DataCollectionApproved { get; set; }
    }
}