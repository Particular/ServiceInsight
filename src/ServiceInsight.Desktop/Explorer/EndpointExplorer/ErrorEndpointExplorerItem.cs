namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
    using System.Drawing;
    using Models;
    using Properties;

    public class ErrorEndpointExplorerItem : EndpointExplorerItem
    {
        public ErrorEndpointExplorerItem(Endpoint endpoint) : base(endpoint)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeErrorQueue; }
        }
    }
}