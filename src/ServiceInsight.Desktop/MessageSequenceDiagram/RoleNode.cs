namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;

    public class RoleNode : DiagramNode
    {
        const int nodeHeaderHeight = 16;
        const int nodeHeaderWidth = 150;

        public RoleNode()
        {
            IsResizable = false;
            Bounds = new Rect(0, 0, nodeHeaderWidth, nodeHeaderHeight);
        }
    }
}
