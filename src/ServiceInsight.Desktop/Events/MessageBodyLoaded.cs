using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class MessageBodyLoaded
    {
        public MessageBodyLoaded(StoredMessage message)
        {
            Message = message;
        }

        public StoredMessage Message { get; private set; }
    }
}