namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System.Drawing;
    using global::ServiceInsight.Properties;
    using Models;

    public class ErrorEndpointExplorerItem : EndpointExplorerItem
    {
        public ErrorEndpointExplorerItem(Endpoint endpoint)
            : base(endpoint)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeErrorQueue; }
        }
    }
}