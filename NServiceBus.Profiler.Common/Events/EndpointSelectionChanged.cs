using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class EndpointSelectionChanged
    {
        public EndpointSelectionChanged(Endpoint endpoint)
        {
            SelectedEndpoint = endpoint;
        }

        public Endpoint SelectedEndpoint { get; private set; }
    }
}