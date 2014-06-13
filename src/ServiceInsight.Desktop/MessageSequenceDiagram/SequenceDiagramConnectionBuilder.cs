namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using Mindscape.WpfDiagramming.Foundation;
    
    public class SequenceDiagramConnectionBuilder : IDiagramConnectionBuilder
    {
        public bool CanCreateConnection(IDiagramModel diagram, IDiagramConnectionPoint fromConnectionPoint, ConnectionDropTarget dropTarget)
        {
            return false;
        }

        public void CreateConnection(IDiagramModel diagram, IDiagramConnectionPoint fromConnectionPoint, IDiagramConnectionPoint toConnectionPoint)
        {
        }
    }
}