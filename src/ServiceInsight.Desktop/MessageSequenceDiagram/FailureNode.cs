namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;
    
    public class FailureNode : DiagramNode
    {
        const int sequenceStartHeight = 16;
        const int sequenceEndWidth = 100;

        public FailureNode()
        {
            IsResizable = false;

            Bounds = new Rect(0, 0, sequenceEndWidth, sequenceStartHeight);
        }
    }
}
