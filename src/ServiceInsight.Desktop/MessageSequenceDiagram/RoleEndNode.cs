using Mindscape.WpfDiagramming;
using System.Windows;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    public class RoleEndNode : DiagramNode
    {
        const int endNodeSize = 1;

        public RoleEndNode()
        {
            IsResizable = false;
            Bounds = new Rect(0, 0, endNodeSize, endNodeSize);
        }   
    }
}
