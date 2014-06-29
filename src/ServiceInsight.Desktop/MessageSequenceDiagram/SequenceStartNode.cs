using Mindscape.WpfDiagramming;
using System.Windows;

namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    public class SequenceStartNode : DiagramNode
    {
        const double sequenceStartNodeHeight = 15.0;
        const double sequenceStartNodeWidth = 14.0;

        public SequenceStartNode()
        {
            IsResizable = false;

            Bounds = new Rect(0.0, 0.0, sequenceStartNodeWidth, sequenceStartNodeHeight);
        }
    }
}
