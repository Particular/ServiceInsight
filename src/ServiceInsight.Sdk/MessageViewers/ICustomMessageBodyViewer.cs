using ServiceInsight.Models;
using ServiceInsight.ServiceControl;

namespace ServiceInsight.MessageViewers
{
    public interface ICustomMessageBodyViewer : IDisplayMessageBody
    {
        bool IsVisible(StoredMessage selectedMessage, PresentationHint presentationHint);
    }
}
