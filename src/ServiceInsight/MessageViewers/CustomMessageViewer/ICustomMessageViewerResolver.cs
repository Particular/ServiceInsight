namespace ServiceInsight.MessageViewers.CustomMessageViewer
{
    public interface ICustomMessageViewerResolver
    {
        ICustomMessageBodyViewer GetCustomMessageBodyViewer();
    }
}