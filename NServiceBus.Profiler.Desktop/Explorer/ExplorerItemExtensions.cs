using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;

namespace NServiceBus.Profiler.Desktop.Explorer
{
    public static class ExplorerItemExtensions
    {
        public static T As<T>(this ExplorerItem explorerItem) where T : ExplorerItem
        {
            return explorerItem as T;
        }

        public static bool IsQueueExplorerSelected(this ExplorerItem explorerItem)
        {
            return explorerItem is QueueExplorerItem ||
                   explorerItem is QueueServerExplorerItem;
        }

        public static bool IsEndpointExplorerSelected(this ExplorerItem explorerItem)
        {
            return explorerItem is EndpointExplorerItem ||
                   explorerItem is ServiceExplorerItem;
        }
    }
}