namespace ServiceInsight.MessagePayloadViewer
{
    using Caliburn.Micro;

    public class MessagePayloadViewModel : Screen
    {
        public string Content { get; private set; }

        public MessagePayloadViewModel(string content)
        {
            Content = content;
        }
    }
}