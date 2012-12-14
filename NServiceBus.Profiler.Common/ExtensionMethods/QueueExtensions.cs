using System.Messaging;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.ExtensionMethods
{
    public static class QueueExtensions
    {
        public static MessageQueue AsMessageQueue(this Queue queue, QueueAccessMode mode = QueueAccessMode.Receive)
        {
            return new MessageQueue(queue.FormatName, mode);
        }
    }
}