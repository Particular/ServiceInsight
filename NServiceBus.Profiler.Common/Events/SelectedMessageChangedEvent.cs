using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class SelectedMessageChangedEvent
    {
        public SelectedMessageChangedEvent(MessageInfo message)
        {
            SelectedMessage = message;
        }

        public MessageInfo SelectedMessage { get; private set; }
    }
}