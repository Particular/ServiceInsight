namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    public interface IQueueConnectionProvider
    {
        string ConnectedToAddress { get; }
    }
}