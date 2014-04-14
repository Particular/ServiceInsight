using System.Messaging;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.ExtensionMethods
{
    public static class QueueExtensions
    {
        public static MessageQueue AsMessageQueue(this Queue queue, QueueAccessMode mode = QueueAccessMode.Receive)
        {
            return new MessageQueue(queue.FormatName, mode);
        }
    }
}