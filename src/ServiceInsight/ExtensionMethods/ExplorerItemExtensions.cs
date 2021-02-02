namespace ServiceInsight.ExtensionMethods
{
    using ServiceInsight.Explorer;
    using ServiceInsight.Explorer.EndpointExplorer;
    using ServiceInsight.ServiceControl;

    public static class ExplorerItemExtensions
    {
        public static IServiceControl GetServiceControlClient(this ExplorerItem item, ServiceControlClientRegistry clientRegistry)
        {
            if (item is AuditEndpointExplorerItem auditEndpointExplorerItem)
            {
                var parent = auditEndpointExplorerItem.ServiceControl;
                var url = parent.Url;

                return clientRegistry.GetServiceControl(url);
            }

            if (item is ServiceControlExplorerItem serviceControlItem)
            {
                return clientRegistry.GetServiceControl(serviceControlItem.Url);
            }

            return null;
        }
    }
}