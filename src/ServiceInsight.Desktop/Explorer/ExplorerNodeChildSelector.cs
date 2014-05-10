namespace Particular.ServiceInsight.Desktop.Explorer
{
    using System.Collections;
    using DevExpress.Xpf.Grid;

    public class ExplorerNodeChildSelector : IChildNodesSelector
    {
        public IEnumerable SelectChildren(object item)
        {
            if (item is ExplorerItem)
                return ((ExplorerItem) item).Children;
            
            return null;
        }
    }
}