using NServiceBus.Profiler.Common.Models;
using QuickGraph;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class MessageEdge : Edge<StoredMessage>
    {
        public MessageEdge(StoredMessage source, StoredMessage target) 
            : base(source, target)
        {
        }
    }
}