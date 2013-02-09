using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class AuditQueueSelectedEvent
    {
        public Endpoint Endpoint { get; set; }
    }
}