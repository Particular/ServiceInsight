using System.Messaging;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    public static class QueueExtensions
    {
        public static MessageQueue AsMessageQueue(this Queue queue, QueueAccessMode mode = QueueAccessMode.Receive)
        {
            return new MessageQueue(queue.FormatName, mode);
        }
    }
}