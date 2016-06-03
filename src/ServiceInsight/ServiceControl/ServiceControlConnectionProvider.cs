namespace ServiceInsight.ServiceControl
{
    public class ServiceControlConnectionProvider
    {
        public void ConnectTo(string url)
        {
            Url = url;
        }

        public string Url { get; private set; }
    }
}