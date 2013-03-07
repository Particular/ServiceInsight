using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    public class ServerExplorerItem : ExplorerItem
    {
        public ServerExplorerItem(string name) : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.Computer; }
        }
    }

}