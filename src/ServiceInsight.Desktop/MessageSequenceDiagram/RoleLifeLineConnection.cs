using Mindscape.WpfDiagramming;

namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    public class RoleLifelineConnection : DiagramConnection
    {
        public RoleLifelineConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10.0;
            IsUserModified = false;
        }
    }
}
