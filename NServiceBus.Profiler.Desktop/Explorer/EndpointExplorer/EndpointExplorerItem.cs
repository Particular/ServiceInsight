using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer
{
    public abstract class EndpointExplorerItem : ExplorerItem
    {
        protected EndpointExplorerItem(Endpoint endpoint) : base(endpoint.Name)
        {
            Endpoint = endpoint;
        }

        public Endpoint Endpoint { get; private set; }
    }
}