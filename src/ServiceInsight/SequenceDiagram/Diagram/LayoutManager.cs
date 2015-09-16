namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System.Collections.Generic;
    using System.Linq;

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
            var endpointTimelineLayout = new EndpointTimelineLayout(diagram);
            var arrowLayout = new ArrowLayout(diagram, endpointLayout);

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
                    endpointTimelineLayout.Position(timeline);
                    continue;
                }

                var handler = item as Handler;
                if (handler != null)
                {
                    handlerLayout.Position(handler);
                    continue;
                }

                var arrow = item as Arrow;
                if (arrow != null)
                {
                    arrowLayout.Position(arrow);
                    continue;
                }
            }

        }

        class ArrowLayout
        {
            readonly IDiagram diagram;
            readonly EndpointItemLayout endpointItemLayout;

            public ArrowLayout(IDiagram diagram, EndpointItemLayout endpointItemLayout)
            {
                this.diagram = diagram;
                this.endpointItemLayout = endpointItemLayout;
            }

            public void Position(Arrow arrow)
            {
                var fromEndpointIndex = 0;
                var fromHandler = arrow.FromHandler;

                if (fromHandler != null)
                {
                    fromEndpointIndex = endpointItemLayout.GetIndexPosition(fromHandler.Endpoint);
                }
                else
                {
                    fromHandler = endpointItemLayout.GetFirst().Handlers.First();
                }

                var toEndpointIndex = endpointItemLayout.GetIndexPosition(arrow.ToHandler.Endpoint);

                var arrowVisual = diagram.GetItemFromContainer(arrow);
                var fromHandlerVisual = diagram.GetItemFromContainer(fromHandler);
                var toHandlerVisual = diagram.GetItemFromContainer(arrow.ToHandler);
                arrowVisual.X = fromHandlerVisual.X;
                var arrowIndex = arrow.FromHandler.Out.IndexOf(arrow) + 1;
                arrowVisual.Y = fromHandlerVisual.Y + ((fromHandlerVisual.ActualHeight / (fromHandler.Out.Count + 1)) * arrowIndex);

                if (fromEndpointIndex == toEndpointIndex)
                {
                    //Local
                    arrow.Direction = Direction.Right;
                    arrowVisual.X += fromHandlerVisual.ActualWidth;

                    var fromEndpointVisual = diagram.GetItemFromContainer(fromHandler.Endpoint);
                    arrow.Width = fromEndpointVisual.ActualWidth/4;
                }
                else if (fromEndpointIndex < toEndpointIndex)
                {
                    //From left to right
                    arrow.Direction = Direction.Right;
                    arrowVisual.X += fromHandlerVisual.ActualWidth;
                    arrow.Width = toHandlerVisual.X - (fromHandlerVisual.X + fromHandlerVisual.ActualWidth);
                }
                else
                {
                    // from right to left
                    arrow.Direction = Direction.Left;
                    arrow.Width = (toHandlerVisual.X + toHandlerVisual.Width) - fromHandlerVisual.X;
                }
            }
        }

        class EndpointTimelineLayout
        {
            IDiagram diagram;

            public EndpointTimelineLayout(IDiagram diagram)
            {
                this.diagram = diagram;
            }

            public void Position(EndpointTimeline timeline)
            {
                var timelineVisual = diagram.GetItemFromContainer(timeline);
                var endpointVisual = diagram.GetItemFromContainer(timeline.Endpoint);

                timelineVisual.X = endpointVisual.X + endpointVisual.ActualWidth / 2;
                timelineVisual.Y = endpointVisual.Y + endpointVisual.ActualHeight;
            }
        }

        class HandlerLayout
        {
            IDiagram diagram;
            double nextY;

            public HandlerLayout(IDiagram diagram)
            {
                this.diagram = diagram;
                nextY = 150d;
            }

            public void Position(Handler handler)
            {
                var handlerVisual = diagram.GetItemFromContainer(handler);
                var endpointVisual = diagram.GetItemFromContainer(handler.Endpoint);

                handlerVisual.X = endpointVisual.X + endpointVisual.ActualWidth / 2 - 7;

                var height = (handler.Out.Count == 0 ? 1 : handler.Out.Count)*25;
                handlerVisual.Height = height;

                handlerVisual.Y = nextY;

                nextY += height + 20;
            }
        }

        class EndpointItemLayout
        {
            IDiagram diagram;
            DiagramVisualItem lastEndpoint;
            double firstX;
            double maxHeight;
            int index;
            Dictionary<EndpointItem, int> position = new Dictionary<EndpointItem, int>();

            public EndpointItemLayout(IDiagram diagram)
            {
                this.diagram = diagram;
                this.maxHeight = GetMaxHeight();
            }

            public void Position(EndpointItem endpoint)
            {
                var endpointVisual = diagram.GetItemFromContainer(endpoint);

                if (index == 0)
                {
                    firstX = endpointVisual.X;
                }

                if (lastEndpoint != null)
                {
                    endpointVisual.X = firstX + ((lastEndpoint.ActualWidth) * index);
                    endpointVisual.Y = lastEndpoint.Y;
                }

                if (endpointVisual.ActualHeight < maxHeight)
                {
                    endpointVisual.Height = maxHeight;
                }

                lastEndpoint = endpointVisual;
                position[endpoint] = index;
                index++;
            }

            public int GetIndexPosition(EndpointItem endpoint)
            {
                return position[endpoint];
            }

            public EndpointItem GetFirst()
            {
                return position.Single(kv => kv.Value == 0).Key;
            }

            private double GetMaxHeight()
            {
                var endpoints = diagram.DiagramItems.OfType<EndpointItem>().ToList();
                var maxHeight = endpoints.Select(endpoint => diagram.GetItemFromContainer(endpoint))
                                         .Max(endpointVisual => endpointVisual.ActualHeight);

                return maxHeight;
            }
        }
    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }

}
