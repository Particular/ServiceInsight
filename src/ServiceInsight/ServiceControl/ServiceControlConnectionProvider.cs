namespace ServiceInsight.ServiceControl
{
    using ServiceInsight.Framework;

    public class ServiceControlConnectionProvider
    {
        IRxServiceControl serviceControl;

        public ServiceControlConnectionProvider(IRxServiceControl serviceControl)
        {
            this.serviceControl = serviceControl;
        }

        public void ConnectTo(string url)
        {
            Url = url;

            if (url == null)
            {
                return;
            }

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