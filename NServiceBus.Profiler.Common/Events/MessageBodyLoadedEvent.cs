using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class MessageBodyLoadedEvent
    {
        public MessageBodyLoadedEvent(MessageBody message)
        {
            Message = message;
        }

        public MessageBody Message { get; private set; }
    }
}