namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Linq;

    public class SequenceDiagramLayoutManager : ILayoutManager
    {
        public void PerformLayout(IDiagram diagram)
        {
            LayoutEndpoints(diagram);
        }

        private void LayoutEndpoints(IDiagram diagram)
        {
            if (diagram.Items == null || diagram.Items.Count == 0)
            {
                return;
            }
            
            var endpoints = diagram.Items.OfType<EndpointItem>().ToList();
            
            EndpointItem lastEndpoint = null;
            var firstX = 0d;
            var index = 0;
            
            foreach (var endpoint in endpoints)
            {
                if (index == 0)
                {
                    firstX = endpoint.X;
                }
            
                if (lastEndpoint != null)
                {
                    endpoint.X = firstX + ((lastEndpoint.Width + 10 /*Margin*/) * index);
                    endpoint.Y = lastEndpoint.Y;
                }
            
                lastEndpoint = endpoint;
                index++;
            }

            var timelines = diagram.Items.OfType<EndpointTimeline>().ToList();
            foreach (var timeline in timelines)
            {
                timeline.X = timeline.Endpoint.X + timeline.Endpoint.Width/2;
                timeline.Y = timeline.Endpoint.Y + timeline.Endpoint.Height + 5;
            }
        }
    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }

}