using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public class ErrorQueueExplorerItem : ExplorerItem
    {
        public ErrorQueueExplorerItem(string name) : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeErrorQueue; }
        }
    }
}