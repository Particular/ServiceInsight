namespace Particular.ServiceInsight.Desktop.Explorer
{
    using EndpointExplorer;

    public static class ExplorerItemExtensions
    {
        public static T As<T>(this ExplorerItem explorerItem) where T : ExplorerItem
        {
            return explorerItem as T;
        }

        public static bool IsEndpointExplorerSelected(this ExplorerItem explorerItem)
        {
            return explorerItem is EndpointExplorerItem ||
                   explorerItem is ServiceControlExplorerItem;
        }
    }
}