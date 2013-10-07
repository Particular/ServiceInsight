using System.Windows;
using Mindscape.WpfDiagramming;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public class MessageNode : DiagramNode
    {
        public MessageNode(StoredMessage input)
        {
            Bounds = new Rect(100, 100, 300, 90);
            ZOrder = 1;
            Data = input;
            IsResizable = true;
        }

        public StoredMessage Message
        {
            get { return Data as StoredMessage; }
        }

        public bool IsCurrentMessage { get; set; }
    }
}