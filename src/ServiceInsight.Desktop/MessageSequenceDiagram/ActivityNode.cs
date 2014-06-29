namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;
    using System.Windows;

    public class ActivityNode : DiagramNode
    {
        const double heightActivity = 30.0;
        const double widthActivity = 10.0;

        public ActivityNode()
        {
            IsResizable = false;
            Bounds = new Rect(0.0, 0.0, widthActivity, heightActivity);
        }
    }
}
