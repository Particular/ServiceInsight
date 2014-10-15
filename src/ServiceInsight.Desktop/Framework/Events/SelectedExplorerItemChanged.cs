namespace Particular.ServiceInsight.Desktop.Framework.Events
{
    using Particular.ServiceInsight.Desktop.Explorer;

    public class SelectedExplorerItemChanged
    {
        public SelectedExplorerItemChanged(ExplorerItem explorerItem)
        {
            SelectedExplorerItem = explorerItem;
        }

        public ExplorerItem SelectedExplorerItem { get; private set; }
    }
}