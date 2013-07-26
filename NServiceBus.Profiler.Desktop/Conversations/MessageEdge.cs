using NServiceBus.Profiler.Desktop.Models;
using QuickGraph;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class MessageEdge : Edge<DiagramNode>
    {
        public MessageEdge(DiagramNode source, DiagramNode target) 
            : base(source, target)
        {
        }
    }
}