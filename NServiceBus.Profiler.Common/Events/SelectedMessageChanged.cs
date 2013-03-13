using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Common.Events
{
    public class SelectedMessageChanged
    {
        public SelectedMessageChanged(MessageInfo message)
        {
            SelectedMessage = message;
        }

        public MessageInfo SelectedMessage { get; private set; }
    }
}