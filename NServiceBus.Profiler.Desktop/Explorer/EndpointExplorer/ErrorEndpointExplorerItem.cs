using System.Drawing;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
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