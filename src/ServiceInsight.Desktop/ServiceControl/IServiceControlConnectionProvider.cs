namespace Particular.ServiceInsight.Desktop.ServiceControl
{
    public interface IServiceControlConnectionProvider
    {
        string Url { get; }
        void ConnectTo(string url);
    }
}