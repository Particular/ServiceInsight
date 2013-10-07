using Mindscape.WpfDiagramming;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public class FlowDiagramConnection : DiagramConnection
    {
        public FlowDiagramConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }
}