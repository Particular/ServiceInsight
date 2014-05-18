using Mindscape.WpfDiagramming;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
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

    public class SDMessageConnection : DiagramConnection
    {
        public SDMessageConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }

    public class SDEventConnection : DiagramConnection
    {
        public SDEventConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
            : base(fromConnectionPoint, toConnectionPoint)
        {
            LineType = null;
            ZOrder = 10;
            IsUserModified = false;
        }
    }
}
