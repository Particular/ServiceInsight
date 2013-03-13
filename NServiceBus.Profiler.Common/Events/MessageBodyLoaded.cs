using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class MessageBodyLoaded
    {
        public MessageBodyLoaded(MessageBody message)
        {
            Message = message;
        }

        public MessageBody Message { get; private set; }
    }
}