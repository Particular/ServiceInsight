using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class MessageRemovedFromQueueEvent
    {
        public MessageInfo Message { get; set; }
    }
}