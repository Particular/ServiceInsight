namespace Particular.ServiceInsight.Desktop.SequenceDiagram.Diagram
{
    public class Endpoint : DiagramItem
    {
        public Endpoint()
        {
            Timeline = new EndpointTimeline();
        }

        public EndpointTimeline Timeline { get; set; }
    }
}