namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System.Messaging;
    using Models;

    public static class QueueExtensions
    {
        public static MessageQueue AsMessageQueue(this Queue queue, QueueAccessMode mode = QueueAccessMode.Receive)
        {
            return new MessageQueue(queue.FormatName, mode);
        }
    }
}