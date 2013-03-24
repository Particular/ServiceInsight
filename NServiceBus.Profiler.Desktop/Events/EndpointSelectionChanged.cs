using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Events
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