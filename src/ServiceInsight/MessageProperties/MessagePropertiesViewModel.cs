namespace ServiceInsight.MessageProperties
{
    using System.ComponentModel;
    using Caliburn.Micro;
    using Framework;

    public class MessagePropertiesViewModel : Screen
    {
        readonly IClipboard clipboard;

        public MessagePropertiesViewModel(
            ErrorHeaderViewModel error,
            GeneralHeaderViewModel general,
            SagaHeaderViewModel saga,
            PerformanceHeaderViewModel performance,
            GatewayHeaderViewModel gateway,
            IClipboard clipboard)
        {
            Saga = saga;
            Performance = performance;
            Gateway = gateway;
            Errors = error;
            General = general;
            this.clipboard = clipboard;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GeneralHeaderViewModel General { get; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PerformanceHeaderViewModel Performance { get; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ErrorHeaderViewModel Errors { get; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GatewayHeaderViewModel Gateway { get; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SagaHeaderViewModel Saga { get; }

        public void CopyPropertyValue(object value)
        {
            clipboard.CopyTo(value.ToString());
        }
    }
}