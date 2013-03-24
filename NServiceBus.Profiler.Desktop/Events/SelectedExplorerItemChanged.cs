using NServiceBus.Profiler.Desktop.Explorer;

namespace NServiceBus.Profiler.Desktop.Events
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