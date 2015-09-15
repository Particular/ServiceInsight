namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Collections.Generic;

    public class SequenceDiagramLayoutManager : ILayoutManager
    {
        public void PerformLayout(IDiagram diagram)
        {
            if (diagram.DiagramItems == null || diagram.DiagramItems.Count == 0)
            {
                return;
            }

            var endpointLayout = new EndpointItemLayout(diagram);
            var handlerLayout = new HandlerLayout(diagram);

            foreach (var item in diagram.DiagramItems)
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
                    var timelineVisual = diagram.GetItemFromContainer(timeline);
                    var endpointVisual = diagram.GetItemFromContainer(timeline.Endpoint);

                    timelineVisual.X = endpointVisual.X + endpointVisual.ActualWidth / 2;
                    timelineVisual.Y = endpointVisual.Y + endpointVisual.ActualHeight + 5;

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
            IDiagram diagram;
            Dictionary<EndpointItem, double> endpointYs = new Dictionary<EndpointItem, double>();

            public HandlerLayout(IDiagram diagram)
            {
                this.diagram = diagram;
            }

            public void Position(Handler handler)
            {
                var handlerVisual = diagram.GetItemFromContainer(handler);
                var endpointVisual = diagram.GetItemFromContainer(handler.Endpoint);

                handlerVisual.X = endpointVisual.X + endpointVisual.ActualWidth / 2 - 7;

                var height = (handler.Out.Count == 0 ? 1 : handler.Out.Count)*25;
                handlerVisual.Height = height;

                if (endpointYs.ContainsKey(handler.Endpoint))
                {
                    endpointYs[handler.Endpoint] += 30 + height;
                    handlerVisual.Y = endpointYs[handler.Endpoint];
                }
                else
                {
                    endpointYs[handler.Endpoint] = handlerVisual.Y = 50 + height;
                }
            }
        }

        class EndpointItemLayout
        {
            IDiagram diagram;
            DiagramVisualItem lastEndpoint;
            double firstX;
            int index;

            public EndpointItemLayout(IDiagram diagram)
            {
                this.diagram = diagram;
            }

            public void Position(DiagramItem endpoint)
            {
                var endpointVisual = diagram.GetItemFromContainer(endpoint);

                if (index == 0)
                {
                    firstX = endpointVisual.X;
                }

                if (lastEndpoint != null)
                {
                    endpointVisual.X = firstX + ((lastEndpoint.ActualWidth + lastEndpoint.Margin.Right) * index);
                    endpointVisual.Y = lastEndpoint.Y;
                }

                lastEndpoint = endpointVisual;
                index++;
            }
        }
    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }

}