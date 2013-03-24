using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class MessageRemovedFromQueue
    {
        public MessageInfo Message { get; set; }
    }
}