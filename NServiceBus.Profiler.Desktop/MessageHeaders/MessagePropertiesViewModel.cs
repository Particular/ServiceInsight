using System.ComponentModel;
using Caliburn.PresentationFramework.Screens;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    [TypeConverter(typeof(HeaderInfoTypeConverter))]
    public class MessagePropertiesViewModel : Screen, IMessagePropertiesViewModel
    {
        public MessagePropertiesViewModel(
            IErrorHeaderViewModel error,
            IGeneralHeaderViewModel general,
            ISagaHeaderViewModel saga,
            IPerformanceHeaderViewModel performance,
            IGatewayHeaderViewModel gateway)
        {
            Saga = saga;
            Performance = performance;
            Gateway = gateway;
            Errors = error;
            General = general;
        }

        [PropertyOrder(1)]
        public IGeneralHeaderViewModel General { get; private set; }

        [PropertyOrder(2)]
        public IPerformanceHeaderViewModel Performance { get; private set; }

        [PropertyOrder(3)]
        public IErrorHeaderViewModel Errors { get; private set; }

        [PropertyOrder(4)]
        public IGatewayHeaderViewModel Gateway { get; private set; }

        [PropertyOrder(5)]
        public ISagaHeaderViewModel Saga { get; private set; }
    }
}