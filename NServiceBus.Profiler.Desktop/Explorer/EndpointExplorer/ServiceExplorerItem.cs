using System.Drawing;
using System.Linq;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Desktop.Properties;

namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    public class ServiceExplorerItem : ExplorerItem
    {
        public ServiceExplorerItem(string name)
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
    }
}