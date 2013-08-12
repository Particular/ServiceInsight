using System.Collections;
using DevExpress.Xpf.Grid;

namespace Particular.ServiceInsight.Desktop.Explorer
{
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