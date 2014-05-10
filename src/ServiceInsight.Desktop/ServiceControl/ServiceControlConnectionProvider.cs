namespace Particular.ServiceInsight.Desktop.ServiceControl
{
    public class ServiceControlConnectionProvider : IServiceControlConnectionProvider
    {
        public void ConnectTo(string url)
        {
            Url = url;
        }

        public string Url { get; private set; }
    }
}