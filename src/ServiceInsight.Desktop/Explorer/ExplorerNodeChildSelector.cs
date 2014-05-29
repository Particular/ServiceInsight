namespace Particular.ServiceInsight.Desktop.Explorer
{
    using System.Collections;
    using DevExpress.Xpf.Grid;

    public class ExplorerNodeChildSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            var explorerItem = item as ExplorerItem;
            return explorerItem != null ? explorerItem.Children : null;
        }
    }
}