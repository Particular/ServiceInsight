using Particular.ServiceInsight.Desktop.Models;
using QuickGraph;

namespace Particular.ServiceInsight.Desktop.Conversations
{
    public class MessageEdge : Edge<DiagramNode>
    {
        public MessageEdge(DiagramNode source, DiagramNode target) 
            : base(source, target)
        {
        }
    }
}