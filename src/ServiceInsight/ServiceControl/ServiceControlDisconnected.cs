namespace ServiceInsight.ServiceControl
{
    using ServiceInsight.Explorer.EndpointExplorer;

    public class ServiceControlDisconnected
    {
        public string ServiceUrl => ExplorerItem.Url;
        public ServiceControlExplorerItem ExplorerItem { get; set; }
    }
}