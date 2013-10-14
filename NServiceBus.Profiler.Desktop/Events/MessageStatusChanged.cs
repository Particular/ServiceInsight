using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class MessageStatusChanged
    {
        public string MessageId { get; private set; }
        public MessageStatus Status { get; private set; }

        public MessageStatusChanged(string messageId, MessageStatus status)
        {
            MessageId = messageId;
            Status = status;
        }
    }
}