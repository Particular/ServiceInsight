using System.Drawing;
using Particular.ServiceInsight.Desktop.Properties;

namespace Particular.ServiceInsight.Desktop.Explorer
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