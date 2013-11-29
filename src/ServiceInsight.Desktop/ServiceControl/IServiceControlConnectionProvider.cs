namespace NServiceBus.Profiler.Desktop.ServiceControl
{
    public interface IServiceControlConnectionProvider
    {
        string Url { get; }
        void ConnectTo(string url);
    }
}