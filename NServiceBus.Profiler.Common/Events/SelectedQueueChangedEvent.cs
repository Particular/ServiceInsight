using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class SelectedQueueChangedEvent
    {
        public SelectedQueueChangedEvent(Queue queue)
        {
            SelectedQueue = queue;
        }

        public Queue SelectedQueue { get; private set; }
    }
}