namespace Particular.ServiceInsight.Desktop.Events
{
    using Models;

    public class SelectedMessageChanged
    {
        public SelectedMessageChanged(StoredMessage message)
        {
            Message = message;
        }

        public StoredMessage Message { get; private set; }
    }
}