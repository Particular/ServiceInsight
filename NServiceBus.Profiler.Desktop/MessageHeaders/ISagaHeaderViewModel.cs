namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    public interface ISagaHeaderViewModel : IHeaderInfoViewModel
    {
        string SagaType { get; }
        string SagaDataType { get; }
        string OriginatingSagaId { get; }
        string OriginatingSagaType { get; }
        string IsSagaTimeoutMessage { get; }
        string SagaId { get; }
    }
}