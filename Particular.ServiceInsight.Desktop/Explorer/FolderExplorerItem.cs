using System.Drawing;
using Particular.ServiceInsight.Desktop.Properties;

namespace Particular.ServiceInsight.Desktop.Explorer
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
    }
}