namespace NServiceBus.Profiler.Desktop.Management
{
    public interface IServiceControlConnectionProvider
    {
        string Url { get; }
        void ConnectTo(string url);
    }
}