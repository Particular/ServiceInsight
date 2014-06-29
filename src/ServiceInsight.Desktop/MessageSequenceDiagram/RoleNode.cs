namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;

    public class RoleNode : DiagramNode
    {
        const double nodeHeaderHeight = 16.0;
        public const double nodeHeaderWidth = 150.0;

        public RoleNode()
        {
            IsResizable = false;
            Bounds = new Rect(0.0, 0.0, nodeHeaderWidth, nodeHeaderHeight);
        }
    }
}
