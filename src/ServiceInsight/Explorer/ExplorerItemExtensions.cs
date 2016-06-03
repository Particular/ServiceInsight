namespace ServiceInsight.Explorer
{
    using EndpointExplorer;

    public static class ExplorerItemExtensions
    {
        public static bool IsEndpointExplorerSelected(this ExplorerItem explorerItem) => explorerItem is EndpointExplorerItem ||
       explorerItem is ServiceControlExplorerItem;
    }
}