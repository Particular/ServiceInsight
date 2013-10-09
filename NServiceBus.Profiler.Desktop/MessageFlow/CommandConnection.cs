using System.Collections.Generic;
using System.Windows.Shapes;
using Mindscape.WpfDiagramming;
using Mindscape.WpfDiagramming.Foundation;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
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