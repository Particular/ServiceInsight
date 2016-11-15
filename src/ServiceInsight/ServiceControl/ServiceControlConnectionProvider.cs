namespace ServiceInsight.ServiceControl
{
    using Caliburn.Micro;

    public class ServiceControlConnectionProvider
    {
        IEventAggregator eventAggregator;

        public ServiceControlConnectionProvider(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void ConnectTo(string url)
        {
            Url = url;
            eventAggregator.PublishOnUIThread(new ServiceControlConnectionChanged());
        }

        public string Url { get; private set; }
    }
}