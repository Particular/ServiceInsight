namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using System.ComponentModel;
    using Caliburn.Micro;

    public class MessagePropertiesViewModel : Screen
    {
        public MessagePropertiesViewModel(
            ErrorHeaderViewModel error,
            GeneralHeaderViewModel general,
            SagaHeaderViewModel saga,
            PerformanceHeaderViewModel performance,
            GatewayHeaderViewModel gateway)
        {
            Saga = saga;
            Performance = performance;
            Gateway = gateway;
            Errors = error;
            General = general;
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
            AppServices.Clipboard.CopyTo(value.ToString());
        }
    }
}