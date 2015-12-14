namespace Particular.ServiceInsight.Desktop.Framework.Events
{
    using Particular.ServiceInsight.Desktop.Models;

    public class SelectedMessageChanged
    {
    }

    public class ScrollDiagramItemIntoView
    {
        public ScrollDiagramItemIntoView(StoredMessage message)
        {
            Message = message;
        }

        public StoredMessage Message { get; }
    }
}