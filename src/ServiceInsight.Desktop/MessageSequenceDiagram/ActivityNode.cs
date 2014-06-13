namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;

    public class ActivityNode : DiagramNode
    {
        const int heightActivity = 30;
        const int widthActivity = 10;

        public ActivityNode()
        {
            IsResizable = false;
            Bounds = new Rect(0, 0, widthActivity, heightActivity);
        }
    }
}
