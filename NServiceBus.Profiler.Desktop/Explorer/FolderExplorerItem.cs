using System.Drawing;
using NServiceBus.Profiler.Desktop.Properties;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public class FolderExplorerItem : ExplorerItem
    {
        public FolderExplorerItem(string name) : base(name)
        {
        }

        public override Bitmap Image
        {
            get { return Resources.TreeDocFolder; }
        }

        public override int? ParentId
        {
            get { return null; }
        }
    }
}