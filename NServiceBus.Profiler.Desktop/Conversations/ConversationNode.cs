namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationNode
    {
        public string MessageId { get; set; }
        public string Title { get; set; }
        public string OriginatingEndpoint { get; set; }
        public bool IsErrorMessage { get; set; }
        public string ReceivingEndpoint { get; set; }
        
        public override string ToString()
        {
            return Title;
        }
    }
}