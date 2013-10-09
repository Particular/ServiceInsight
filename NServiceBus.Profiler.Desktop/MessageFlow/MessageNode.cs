using System.Diagnostics;
using System.Windows;
using Mindscape.WpfDiagramming;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    [DebuggerDisplay("Type={Message.FriendlyMessageType}, Id={Message.Id}")]
    public class MessageNode : DiagramNode
    {
        public MessageNode(StoredMessage input)
        {
            Bounds = new Rect(100, 100, 203, 75);
            ZOrder = 1;
            Data = input;
            IsResizable = true;
        }

        public StoredMessage Message
        {
            get { return Data as StoredMessage; }
        }

        public bool DisplayEndpointInformation
        {
            get; set;
        }

        public bool IsPublished
        {
            get { return Message.MessageIntent == MessageIntent.Publish; }
        }

        public bool IsCurrentMessage { get; set; }
    }
}