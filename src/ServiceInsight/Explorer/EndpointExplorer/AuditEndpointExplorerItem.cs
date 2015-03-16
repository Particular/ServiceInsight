namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System.Drawing;
    using global::ServiceInsight.Properties;
    using Models;

    public class AuditEndpointExplorerItem : EndpointExplorerItem
    {
        public AuditEndpointExplorerItem(Endpoint endpoint)
            : base(endpoint)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeAuditQueue; }
        }
    }
}