using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class ErrorQueueSelectedEvent
    {
        public Endpoint Endpoint { get; set; }
    }
}