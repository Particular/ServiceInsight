namespace ServiceInsight.ServiceControl
{
    using Caliburn.Micro;

    public class ServiceControlConnectionProvider
    {
        public ServiceControlConnectionProvider()
        {
        }

        public void ConnectTo(string url)
        {
            Url = url;
            //eventAggregator.PublishOnUIThread(new ServiceControlConnectionChanged());
        }

        public string Url { get; private set; }
    }
}