using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer.QueueExplorer
{
    public class QueueServerExplorerItem : ExplorerItem
    {
        public QueueServerExplorerItem(string name) : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.Computer; }
        }
    }

}