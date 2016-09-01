namespace ServiceInsight.ServiceControl
{
    using ServiceInsight.Framework;

    public class ServiceControlConnectionProvider
    {
        public void ConnectTo(string url)
        {
            Url = url;

            AsyncPump.Run(async () =>
            {
                await RxServiceControl.Instance.ClearServiceControls();
                await RxServiceControl.Instance.AddServiceControl(url);

                // Prime the streams
                await RxServiceControl.Instance.Refresh();
            });
        }

        public string Url { get; private set; }
    }
}