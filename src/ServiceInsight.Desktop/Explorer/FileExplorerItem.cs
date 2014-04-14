using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public class FileExplorerItem : ExplorerItem
    {
        public FileExplorerItem(string name)
            : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeDocFile; }
        }
    }
}