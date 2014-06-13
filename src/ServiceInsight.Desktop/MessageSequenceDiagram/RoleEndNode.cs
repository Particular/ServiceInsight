namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;

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
