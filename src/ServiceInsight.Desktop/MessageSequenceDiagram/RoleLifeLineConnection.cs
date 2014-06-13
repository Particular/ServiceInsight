using Mindscape.WpfDiagramming;

namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    public class RoleLifelineConnection : DiagramConnection
    {
        public RoleLifelineConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }

    public class TimeoutConnection : DiagramConnection
    {
        public TimeoutConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }

    public class MessageConnection : DiagramConnection
    {
        public MessageConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }

    public class EventConnection : DiagramConnection
    {
        public EventConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }
}
