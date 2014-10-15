namespace Particular.ServiceInsight.Desktop.Framework.Events
{
    using Particular.ServiceInsight.Desktop.Models;

    public class SelectedMessageChanged
    {
        public SelectedMessageChanged(StoredMessage message)
        {
            Message = message;
        }

        public StoredMessage Message { get; private set; }
    }
}