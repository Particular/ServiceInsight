namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using Caliburn.PresentationFramework.Screens;

    public interface IMessagePropertiesViewModel : IScreen
    {
        ErrorHeaderViewModel Errors { get; }
        GeneralHeaderViewModel General { get; }
        SagaHeaderViewModel Saga { get; }
        PerformanceHeaderViewModel Performance { get; }
        GatewayHeaderViewModel Gateway { get; }
        void CopyPropertyValue(object value);
    }
}