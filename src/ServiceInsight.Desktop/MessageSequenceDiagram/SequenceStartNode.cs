using Mindscape.WpfDiagramming;
using System.Windows;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    public class SequenceStartNode : DiagramNode
    {
        const int sequenceStartNodeHeight = 15;
        const int sequenceStartNodeWidth = 14;

        public SequenceStartNode()
        {
            IsResizable = false;

            Bounds = new Rect(0, 0, sequenceStartNodeWidth, sequenceStartNodeHeight);
        }
    }
}
