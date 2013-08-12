using System.Windows.Media;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.TreeList;
using Particular.ServiceInsight.Desktop.ExtensionMethods;

namespace Particular.ServiceInsight.Desktop.Explorer
{
    public class ExplorerNodeImageSelector : TreeListNodeImageSelector
    {
        public override ImageSource Select(TreeListRowData rowData)
        {
            var explorerItem = rowData.Node.Content as ExplorerItem;
            if (explorerItem != null)
            {
                return explorerItem.Image.ToBitmapImage();
            }

            return null;
        }
    }
}