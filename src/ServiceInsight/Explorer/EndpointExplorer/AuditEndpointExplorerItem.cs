namespace ServiceInsight.Explorer.EndpointExplorer
{
    using ServiceInsight.Models;

    public class AuditEndpointExplorerItem : EndpointExplorerItem
    {
        public AuditEndpointExplorerItem(ServiceControlExplorerItem parent, Endpoint endpoint, string hostNames = "")
            : base(endpoint)
        {
            ServiceControl = parent;
            HostNames = hostNames;
        }

        public ServiceControlExplorerItem ServiceControl { get; }

        public string HostNames { get; set; }

        public override string Image => "Shell_EndpointTreeNode";
    }
}