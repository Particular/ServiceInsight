using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Events
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