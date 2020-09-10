namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System.Linq;
    using Models;

    public class ServiceControlExplorerItem : ExplorerItem
    {
        public ServiceControlExplorerItem(string name)
            : base(name)
        {
        }

        public override string Image => "Shell_EndpointRootNode";

        public bool EndpointExists(Endpoint endpoint) => Children.Any(item => item.Endpoint == endpoint);

        public EndpointExplorerItem GetEndpointNode(Endpoint endpoint) => Children.First(item => item.Endpoint == endpoint);
    }
}