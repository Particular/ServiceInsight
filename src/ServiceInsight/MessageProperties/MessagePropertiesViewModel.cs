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
        public GeneralHeaderViewModel General { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public PerformanceHeaderViewModel Performance { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ErrorHeaderViewModel Errors { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GatewayHeaderViewModel Gateway { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SagaHeaderViewModel Saga { get; private set; }

        public void CopyPropertyValue(object value)
        {
            clipboard.CopyTo(value.ToString());
        }
    }
}