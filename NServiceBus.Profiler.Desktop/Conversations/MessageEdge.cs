using QuickGraph;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class MessageEdge : Edge<ConversationNode>
    {
        public MessageEdge(ConversationNode source, ConversationNode target) 
            : base(source, target)
        {
        }
    }
}