namespace ServiceInsight.MessageViewers
{
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;

    public interface ICustomMessageBodyViewer : IDisplayMessageBody
    {
        bool IsVisible(StoredMessage selectedMessage, PresentationHint presentationHint);
    }
}
