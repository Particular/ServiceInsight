namespace ServiceInsight.ServiceControl
{
    using Caliburn.Micro;
    using ServiceInsight.Framework;

    public class ServiceControlConnectionProvider
    {
        IEventAggregator eventAggregator;
		IRxServiceControl serviceControl;

        public ServiceControlConnectionProvider(IEventAggregator eventAggregator, IRxServiceControl serviceControl)
        {
            this.eventAggregator = eventAggregator;
			this.serviceControl = serviceControl;
        }

        public void ConnectTo(string url)
        {
            Url = url;
			
			eventAggregator.PublishOnUIThread(new ServiceControlConnectionChanged());

            AsyncPump.Run(async () =>
            {
                await serviceControl.ClearServiceControls();
                await serviceControl.AddServiceControl(url);

                // Prime the streams
                await serviceControl.Refresh();
            });
        }

        public string Url { get; private set; }
    }
}