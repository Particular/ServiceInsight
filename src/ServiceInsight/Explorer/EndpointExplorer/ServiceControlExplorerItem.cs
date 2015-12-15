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

        public override Bitmap Image
        {
            get { return Resources.TreeMonitoring; }
        }

        public bool EndpointExists(Endpoint endpoint)
        {
            return Children.Any(item => item.Endpoint == endpoint);
        }

        public EndpointExplorerItem GetEndpointNode(Endpoint endpoint)
        {
            return Children.First(item => item.Endpoint == endpoint);
        }
    }
}