namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Collections.Generic;

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

            var endpointLayout = new EndpointItemLayout();
            var handlerLayout = new HandlerLayout();

            foreach (var item in diagram.Items)
            {
                var endpoint = item as EndpointItem;

                if (endpoint != null)
                {
                    endpointLayout.Position(endpoint);

                    continue;
                }

                var timeline = item as EndpointTimeline;
                if (timeline != null)
                {
                    timeline.X = timeline.Endpoint.X + timeline.Endpoint.Width / 2;
                    timeline.Y = timeline.Endpoint.Y + timeline.Endpoint.Height + 5;

                    continue;
                }


                var handler = item as Handler;
                if (handler != null)
                {
                    handlerLayout.Position(handler);
                    
                    continue;
                }
            }
        }

        class HandlerLayout
        {
            Dictionary<EndpointItem, double> endpointYs = new Dictionary<EndpointItem, double>();
             
            public void Position(Handler handler)
            {
                handler.X = handler.Endpoint.X + handler.Endpoint.Width / 2 - 7;

                var height = (handler.Out.Count == 0 ? 1 : handler.Out.Count)*25;
                handler.Height = height;

                if (endpointYs.ContainsKey(handler.Endpoint))
                {
                    endpointYs[handler.Endpoint] += 30 + height;
                    handler.Y = endpointYs[handler.Endpoint];
                }
                else
                {
                    endpointYs[handler.Endpoint] = handler.Y = 50 + height;
                }
            }
        }

        class EndpointItemLayout
        {
            EndpointItem lastEndpoint;
            double firstX;
            int index;

            public void Position(EndpointItem endpoint)
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
        }
    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }

}