namespace ServiceInsight.Explorer.EndpointExplorer
{
    using System.Drawing;
    using System.Linq;
    using global::ServiceInsight.Properties;
    using Models;

    public class ServiceControlExplorerItem : ExplorerItem
    {
        public ServiceControlExplorerItem(string name)
            : base(name)
        {
        }

        public override Bitmap Image => Resources.TreeMonitoring;

        public bool EndpointExists(Endpoint endpoint) => Children.Any(item => item.Endpoint == endpoint);

        public EndpointExplorerItem GetEndpointNode(Endpoint endpoint) => Children.First(item => item.Endpoint == endpoint);
    }
}