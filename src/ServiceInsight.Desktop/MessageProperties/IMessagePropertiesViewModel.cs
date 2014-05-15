namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using Caliburn.PresentationFramework.Screens;

    public interface IMessagePropertiesViewModel : IScreen
    {
        ErrorHeaderViewModel Errors { get; }
        IGeneralHeaderViewModel General { get; }
        ISagaHeaderViewModel Saga { get; }
        IPerformanceHeaderViewModel Performance { get; }
        GatewayHeaderViewModel Gateway { get; }
        void CopyPropertyValue(object value);
    }
}