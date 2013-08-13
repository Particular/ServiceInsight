using Caliburn.PresentationFramework.Screens;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    public interface IMessagePropertiesViewModel : IScreen
    {
        IErrorHeaderViewModel Errors { get; }
        IGeneralHeaderViewModel General { get; }
        ISagaHeaderViewModel Saga { get; }
        IPerformanceHeaderViewModel Performance { get; }
        IGatewayHeaderViewModel Gateway { get; }
    }
}