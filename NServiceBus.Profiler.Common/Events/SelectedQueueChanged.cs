using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class SelectedQueueChanged
    {
        public SelectedQueueChanged(Queue queue)
        {
            SelectedQueue = queue;
        }

        public Queue SelectedQueue { get; private set; }
    }
}