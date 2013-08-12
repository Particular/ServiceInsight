using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Events
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