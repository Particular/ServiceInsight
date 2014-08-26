namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;
    
    public class FailureNode : DiagramNode
    {
        const double sequenceStartHeight = 16.0;
        const double sequenceEndWidth = 100.0;

        public FailureNode()
        {
            IsResizable = false;

            Bounds = new Rect(0.0, 0.0, sequenceEndWidth, sequenceStartHeight);
        }
    }
}
