using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public class EndpointExplorerItem : ExplorerItem
    {
        public EndpointExplorerItem(string name)
            : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeMonitoring; }
        }
    }
}