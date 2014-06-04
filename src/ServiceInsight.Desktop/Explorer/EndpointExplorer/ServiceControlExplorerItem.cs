namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System.Drawing;
    using System.Linq;
    using Models;
    using Properties;

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
            return Children.OfType<EndpointExplorerItem>()
                           .Any(item => item.Endpoint == endpoint);
        }

        public EndpointExplorerItem GetEndpointNode(Endpoint endpoint)
        {
            return Children.OfType<EndpointExplorerItem>()
                           .First(item => item.Endpoint == endpoint);
        }
    }
}