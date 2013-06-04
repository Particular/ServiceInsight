using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IMessagePropertiesViewModel : IScreen
    {
        IRawHeaderViewModel RawHeader { get; }
        IErrorHeaderViewModel Errors { get; }
        IGeneralHeaderViewModel General { get; }
        ISagaHeaderViewModel Saga { get; }
        IPerformanceHeaderViewModel Performance { get; }
        IGatewayHeaderViewModel Gateway { get; }
    }
}