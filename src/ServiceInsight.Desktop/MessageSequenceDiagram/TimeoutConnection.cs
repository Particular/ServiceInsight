namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;

    public class TimeoutConnection : DiagramConnection
    {
        public TimeoutConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10.0;
            IsUserModified = false;
        }
    }
}
