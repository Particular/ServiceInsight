using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public abstract class AutoRefreshBeat
    {
    }

    public class EndpointAutoRefreshBeat : AutoRefreshBeat
    {
        public Endpoint SelectedEndpoint { get; set; }
    }

    public class QueueAutoRefreshBeat : AutoRefreshBeat
    {
        public Queue SelectedQueue { get; set; }
    }
}