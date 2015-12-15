namespace ServiceInsight.MessageFlow
{
    using Mindscape.WpfDiagramming;

    public class CommandConnection : DiagramConnection
    {
        public CommandConnection(DiagramConnectionPoint fromConnectionPoint, DiagramConnectionPoint toConnectionPoint)
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