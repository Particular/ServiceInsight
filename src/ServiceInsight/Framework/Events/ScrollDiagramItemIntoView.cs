namespace ServiceInsight.Framework.Events
{
    using ServiceInsight.Models;

    public class ScrollDiagramItemIntoView
    {
        public ScrollDiagramItemIntoView(StoredMessage message)
        {
            Message = message;
        }

        public StoredMessage Message { get; }
    }
}