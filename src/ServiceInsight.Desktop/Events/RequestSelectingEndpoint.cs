using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class RequestSelectingEndpoint
    {
        public Endpoint Endpoint { get; private set; }

        public RequestSelectingEndpoint(Endpoint endpoint)
        {
            Endpoint = endpoint;
        }
    }
}