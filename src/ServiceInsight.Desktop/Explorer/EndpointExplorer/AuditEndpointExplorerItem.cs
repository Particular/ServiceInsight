namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System.Drawing;
    using Models;
    using Properties;

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