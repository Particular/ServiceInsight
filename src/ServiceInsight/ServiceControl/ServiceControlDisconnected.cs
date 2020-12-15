using ServiceInsight.Explorer.EndpointExplorer;

namespace ServiceInsight.ServiceControl
{
    public class ServiceControlDisconnected
    {
        public string ServiceUrl => ExplorerItem.Url;
        public ServiceControlExplorerItem ExplorerItem { get; set; }
    }
}