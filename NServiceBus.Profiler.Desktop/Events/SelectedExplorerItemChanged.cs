using Particular.ServiceInsight.Desktop.Explorer;

namespace Particular.ServiceInsight.Desktop.Events
{
    public class SelectedExplorerItemChanged
    {
        public SelectedExplorerItemChanged(ExplorerItem explorerItem)
        {
            SelectedExplorerItem = explorerItem;
        }

        public ExplorerItem SelectedExplorerItem { get; private set; }
    }
}