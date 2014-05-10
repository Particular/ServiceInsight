namespace Particular.ServiceInsight.Desktop.MessageProperties
{
    using Caliburn.PresentationFramework.Screens;

    public interface IMessagePropertiesViewModel : IScreen
    {
        IErrorHeaderViewModel Errors { get; }
        IGeneralHeaderViewModel General { get; }
        ISagaHeaderViewModel Saga { get; }
        IPerformanceHeaderViewModel Performance { get; }
        IGatewayHeaderViewModel Gateway { get; }
        void CopyPropertyValue(object value);
    }
}