using Mindscape.WpfDiagramming;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    using System.Windows;

    public class RoleNode : DiagramNode
    {
        const int nodeHeaderHeight = 16;
        const int nodeHeaderWidth = 150;

        public RoleNode()
        {
            IsResizable = false;
            Bounds = new Rect(0, 0, nodeHeaderWidth, nodeHeaderHeight);
        }
    }
}
