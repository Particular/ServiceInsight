namespace ServiceInsight.MessageViewers.NopCustomViewer
{
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;

    public class NopViewer : ICustomMessageBodyViewer
    {
        public void Display(StoredMessage selectedMessage)
        {
            
        }

        public void Clear()
        {
            
        }

        public bool IsVisible(StoredMessage selectedMessage, PresentationHint presentationHint)
        {
            return false;
        }
    }
}