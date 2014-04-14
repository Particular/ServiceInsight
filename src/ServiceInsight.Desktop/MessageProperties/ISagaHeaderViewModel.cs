namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface ISagaHeaderViewModel : IHeaderInfoViewModel, IPropertyDataProvider
    {
        string SagaType { get; }
        string SagaDataType { get; }
        string OriginatingSagaId { get; }
        string OriginatingSagaType { get; }
        string IsSagaTimeoutMessage { get; }
        string SagaId { get; }
    }
}