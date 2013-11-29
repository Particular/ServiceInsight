using System.Drawing;
using System.Linq;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
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