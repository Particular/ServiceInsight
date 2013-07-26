using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
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