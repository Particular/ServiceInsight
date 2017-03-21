namespace ServiceInsight.MessagePayloadViewer
{
    using Caliburn.Micro;

    public class MessagePayloadViewModel : Screen
    {
        public string Content { get; }

        public MessagePayloadViewModel(string content)
        {
            Content = content;
        }
    }
}