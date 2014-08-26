namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming;

    public class EventConnection : DiagramConnection
    {
        public EventConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10.0;
            IsUserModified = false;
        }
    }
}
