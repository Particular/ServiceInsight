namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
{
    public interface IQueueConnectionProvider
    {
        string ConnectedToAddress { get; }
    }
}