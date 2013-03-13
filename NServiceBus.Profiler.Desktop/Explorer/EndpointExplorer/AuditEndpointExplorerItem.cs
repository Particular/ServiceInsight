using System.Drawing;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public class AuditEndpointExplorerItem : EndpointExplorerItem
    {
        public AuditEndpointExplorerItem(Endpoint endpoint) : base(endpoint)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeAuditQueue; }
        }
    }
}