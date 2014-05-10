namespace Particular.ServiceInsight.Desktop.Explorer
{
    using System.Drawing;
    using Properties;

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