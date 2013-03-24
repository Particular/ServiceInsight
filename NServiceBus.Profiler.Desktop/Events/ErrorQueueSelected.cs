using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class ErrorQueueSelected
    {
        public Endpoint Endpoint { get; set; }
    }
}