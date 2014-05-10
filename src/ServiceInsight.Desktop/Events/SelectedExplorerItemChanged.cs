namespace Particular.ServiceInsight.Desktop.Events
{
    using Explorer;

    public class SelectedExplorerItemChanged
    {
        public SelectedExplorerItemChanged(ExplorerItem explorerItem)
        {
            SelectedExplorerItem = explorerItem;
        }

        public ExplorerItem SelectedExplorerItem { get; private set; }
    }
}