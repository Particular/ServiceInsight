using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class MessageRemovedFromQueue
    {
        public MessageInfo Message { get; set; }
    }
}