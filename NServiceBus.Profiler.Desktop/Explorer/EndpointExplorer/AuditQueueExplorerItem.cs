using System.Drawing;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public class AuditQueueExplorerItem : ExplorerItem
    {
        public AuditQueueExplorerItem(Endpoint endpoint) : base(endpoint.Name)
        {
            Endpoint = endpoint;
        }

        public override Bitmap Image
        {
            get { return Resources.TreeAuditQueue; }
        }

        public Endpoint Endpoint { get; private set; }
    }
}