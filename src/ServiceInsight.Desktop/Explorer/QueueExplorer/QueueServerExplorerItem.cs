namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
{
    using System.Drawing;
    using Properties;

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