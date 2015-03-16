namespace Particular.ServiceInsight.Desktop.Explorer
{
    using System.Drawing;
    using global::ServiceInsight.Properties;

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