namespace ServiceInsight.Framework.Events
{
    using ServiceInsight.Explorer;

    public class SelectedExplorerItemChanged
    {
        public SelectedExplorerItemChanged(ExplorerItem explorerItem)
        {
            SelectedExplorerItem = explorerItem;
        }

        public ExplorerItem SelectedExplorerItem { get; }
    }
}