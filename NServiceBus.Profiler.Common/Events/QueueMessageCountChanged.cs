using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class QueueMessageCountChanged
    {
        public QueueMessageCountChanged(Queue queue, int count)
        {
            Queue = queue;
            Count = count;
        }

        public int Count { get; private set; }
        public Queue Queue { get; private set; }
    }
}