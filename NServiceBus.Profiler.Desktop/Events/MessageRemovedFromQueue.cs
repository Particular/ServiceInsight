using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class MessageRemovedFromQueue
    {
        public MessageInfo Message { get; set; }
    }
}