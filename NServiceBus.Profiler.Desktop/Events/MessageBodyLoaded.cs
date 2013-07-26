using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
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