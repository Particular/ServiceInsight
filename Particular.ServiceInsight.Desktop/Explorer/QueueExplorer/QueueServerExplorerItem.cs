using System.Drawing;
using Particular.ServiceInsight.Desktop.Properties;

namespace Particular.ServiceInsight.Desktop.Explorer.QueueExplorer
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