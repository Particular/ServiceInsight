using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
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