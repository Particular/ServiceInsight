using System.Drawing;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Desktop.Properties;

namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
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