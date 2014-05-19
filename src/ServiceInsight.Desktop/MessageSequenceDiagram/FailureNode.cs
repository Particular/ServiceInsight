using Mindscape.WpfDiagramming;
using System.Windows;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{

    public class FailureNode : DiagramNode
    {
        const int endNodeSize = 1;

        public FailureNode()
        {
            IsResizable = false;

            Bounds = new Rect(0, 0, endNodeSize, endNodeSize);
        }
    }
}
