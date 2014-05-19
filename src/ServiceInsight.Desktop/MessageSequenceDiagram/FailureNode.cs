using Mindscape.WpfDiagramming;
using System.Windows;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{

    public class FailureNode : DiagramNode
    {
        const int sequenceStartHeight = 16;
        const int sequenceEndWidth = 150;

        public FailureNode()
        {
            IsResizable = false;

            Bounds = new Rect(0, 0, sequenceEndWidth, sequenceStartHeight);
        }
    }
}
