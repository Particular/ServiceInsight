using System.ComponentModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public class MessagePropertiesViewModel : Screen, IMessagePropertiesViewModel
    {
        private readonly IClipboard _clipboard;

        public MessagePropertiesViewModel(
            IErrorHeaderViewModel error,
            IGeneralHeaderViewModel general,
            ISagaHeaderViewModel saga,
            IPerformanceHeaderViewModel performance,
            IGatewayHeaderViewModel gateway,
            IClipboard clipboard)
        {
            _clipboard = clipboard;
            Saga = saga;
            Performance = performance;
            Gateway = gateway;
            Errors = error;
            General = general;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IGeneralHeaderViewModel General { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IPerformanceHeaderViewModel Performance { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IErrorHeaderViewModel Errors { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IGatewayHeaderViewModel Gateway { get; private set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public ISagaHeaderViewModel Saga { get; private set; }

        public void CopyPropertyValue(object value)
        {
            _clipboard.CopyTo(value.ToString());
        }
    }
}