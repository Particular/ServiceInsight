using System.Drawing;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Desktop.Properties;

namespace Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer
{
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