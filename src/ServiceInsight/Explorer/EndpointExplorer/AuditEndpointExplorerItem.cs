namespace ServiceInsight.Explorer.EndpointExplorer
{
    using ServiceInsight.Models;

    public class AuditEndpointExplorerItem : EndpointExplorerItem
    {
        public AuditEndpointExplorerItem(Endpoint endpoint, string hostNames = "")
            : base(endpoint)
        {
            HostNames = hostNames;
        }

        public string HostNames { get; set; }

        public override string Image => "Shell_EndpointTreeNode";
    }
}