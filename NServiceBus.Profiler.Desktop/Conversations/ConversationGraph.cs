using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationGraph : QuickGraph.BidirectionalGraph<DiagramNode, MessageEdge>
    {
    }
}