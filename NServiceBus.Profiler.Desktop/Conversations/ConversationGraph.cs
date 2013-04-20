using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationGraph : QuickGraph.BidirectionalGraph<DiagramNode, MessageEdge>
    {
    }
}