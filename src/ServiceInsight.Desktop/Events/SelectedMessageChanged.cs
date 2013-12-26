using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Events
{
    public class SelectedMessageChanged
    {
        public SelectedMessageChanged(StoredMessage message)
        {
            Message = message;
        }

        public StoredMessage Message { get; private set; }
    }
}