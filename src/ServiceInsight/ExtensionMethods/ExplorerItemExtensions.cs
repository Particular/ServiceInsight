using ServiceInsight.Explorer;
using ServiceInsight.Explorer.EndpointExplorer;
using ServiceInsight.ServiceControl;

namespace ServiceInsight.ExtensionMethods
{
    public static class ExplorerItemExtensions
    {
        public static IServiceControl GetServiceControlClient(this ExplorerItem item, ServiceControlClientRegistry clientRegistry)
        {
            if (item is AuditEndpointExplorerItem)
            {
                var parent = ((AuditEndpointExplorerItem) item).ServiceControl;
                var url = parent.Url;

                return clientRegistry.GetServiceControl(url);
            }

            if (item is ServiceControlExplorerItem)
            {
                var serviceControlItem = (ServiceControlExplorerItem) item;
                return clientRegistry.GetServiceControl(serviceControlItem.Url);
            }

            return null;
        }
    }
}