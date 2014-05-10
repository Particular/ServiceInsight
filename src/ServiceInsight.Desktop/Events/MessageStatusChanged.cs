namespace Particular.ServiceInsight.Desktop.Events
{
    using Models;

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