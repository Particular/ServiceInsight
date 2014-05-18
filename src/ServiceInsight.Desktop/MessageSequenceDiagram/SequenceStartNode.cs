using Mindscape.WpfDiagramming;
using System.Windows;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    public class SequenceStartNode : DiagramNode
    {
        const int sequenceStartNodeSize = 1;

        public SequenceStartNode()
        {
            IsResizable = false;

            Bounds = new Rect(0, 0, sequenceStartNodeSize, sequenceStartNodeSize);
        }
    }
}
