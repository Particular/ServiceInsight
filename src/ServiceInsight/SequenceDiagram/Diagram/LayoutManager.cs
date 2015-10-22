namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Caliburn.Micro;

    public class SequenceDiagramLayoutManager : ILayoutManager
    {
        public void PerformLayout(IDiagram diagram)
        {
            if (diagram.DiagramItems == null || diagram.DiagramItems.Count == 0)
            {
                return;
            }

            var endpointLayout = new EndpointItemLayout(diagram);
            var handlerLayout = new HandlerLayout(diagram, endpointLayout);
            var endpointTimelineLayout = new EndpointTimelineLayout(diagram);
            var arrowLayout = new ArrowLayout(diagram, endpointLayout);
            var processRouteLayout = new ProcessRouteLayout(diagram);

            foreach (var item in diagram.DiagramItems)
            {
                var endpoint = item as EndpointItem;
                if (endpoint != null)
                {
                    endpointLayout.Position(endpoint);
                    continue;
                }

                var handler = item as Handler;
                if (handler != null)
                {
                    handlerLayout.Position(handler);
                    continue;
                }

                var timeline = item as EndpointTimeline;
                if (timeline != null)
                {
                    endpointTimelineLayout.Position(timeline);
                    continue;
                }

                var arrow = item as Arrow;
                if (arrow != null)
                {
                    arrowLayout.Position(arrow);
                }

                var route = item as MessageProcessingRoute;
                if (route != null)
                {
                    processRouteLayout.Position(route);
                }
            }
        }

        class ProcessRouteLayout
        {
            readonly IDiagram diagram;
            const double ArrowBoundary = 5;

            public ProcessRouteLayout(IDiagram diagram)
            {
                this.diagram = diagram;
            }

            public void Position(MessageProcessingRoute route)
            {
                var handler = diagram.GetItemFromContainer(route.ProcessingHandler);
                var arrow = diagram.GetItemFromContainer(route.FromArrow);
                var routeVisual = diagram.GetItemFromContainer(route);
                var height = Math.Abs(arrow.Y - handler.Y);

                routeVisual.X = handler.X + ((handler.ActualWidth - routeVisual.ActualWidth) / 2);
                routeVisual.Y = arrow.Y + ArrowBoundary;
                routeVisual.Height = height - ArrowBoundary;
            }
        }

        class ArrowLayout
        {
            const double ArrowHeadWidth = 3;
            const double ButtonPadding = 30;

            readonly IDiagram diagram;
            readonly EndpointItemLayout endpointItemLayout;

            TextBlock textBlock = new TextBlock();

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
                arrowVisual.Y = fromHandlerVisual.Y + ((fromHandlerVisual.Height/(fromHandler.Out.Count + 1))*arrowIndex) - 8;

                if (fromEndpointIndex == toEndpointIndex)
                {
                    //Local
                    arrow.Direction = Direction.Right;
                    arrowVisual.X += fromHandlerVisual.ActualWidth;

                    var fromEndpointVisual = diagram.GetItemFromContainer(fromHandler.Endpoint);
                    arrow.Width = (fromEndpointVisual.ActualWidth/4) - ArrowHeadWidth;
                }
                else if (fromEndpointIndex < toEndpointIndex)
                {
                    //From left to right
                    arrow.Direction = Direction.Right;
                    arrowVisual.X += fromHandlerVisual.ActualWidth;
                    arrow.Width = toHandlerVisual.X - (fromHandlerVisual.X + fromHandlerVisual.ActualWidth) - ArrowHeadWidth;
                }
                else
                {
                    // from right to left
                    arrow.Direction = Direction.Left;
                    arrowVisual.X = toHandlerVisual.X - MeasureString(arrow.Name).Width - ButtonPadding - ArrowHeadWidth;
                    arrow.Width = fromHandlerVisual.X - (toHandlerVisual.X + toHandlerVisual.ActualWidth) - ArrowHeadWidth;
                }
            }

            Size MeasureString(string text)
            {
                var formattedText = new FormattedText(
                    text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                    textBlock.FontSize,
                    Brushes.Black);

                return new Size(formattedText.Width, formattedText.Height);
            }
        }

        class EndpointTimelineLayout
        {
            IDiagram diagram;
            double maxHeight;

            public EndpointTimelineLayout(IDiagram diagram)
            {
                this.diagram = diagram;
                maxHeight = GetMaxHeight();
            }

            public void Position(EndpointTimeline timeline)
            {
                var timelineVisual = diagram.GetItemFromContainer(timeline);
                var endpointVisual = diagram.GetItemFromContainer(timeline.Endpoint);

                timelineVisual.X = endpointVisual.X + endpointVisual.ActualWidth/2;
                timelineVisual.Y = endpointVisual.Y + endpointVisual.ActualHeight;
                timelineVisual.Height = maxHeight;
            }

            double GetMaxHeight()
            {
                var handlers = diagram.DiagramItems.OfType<Handler>().ToList();
                var height = handlers.Select(handler => diagram.GetItemFromContainer(handler))
                    .Max(visual => visual.Y);

                return height + 50; //Continue a bit from the last handler
            }
        }

        class HandlerLayout
        {
            IDiagram diagram;
            double nextY;

            public HandlerLayout(IDiagram diagram, EndpointItemLayout endpointItemLayout)
            {
                this.diagram = diagram;
                nextY = endpointItemLayout.MaxHeight + 25;
            }

            public void Position(Handler handler)
            {
                var handlerVisual = diagram.GetItemFromContainer(handler);
                var endpointVisual = diagram.GetItemFromContainer(handler.Endpoint);

                handlerVisual.X = endpointVisual.X + endpointVisual.ActualWidth/2 - 7;

                var height = (handler.Out.Count == 0 ? 1 : handler.Out.Count)*25;
                handlerVisual.Height = height;

                handlerVisual.Y = nextY;

                nextY += height + 20;
            }
        }

        class EndpointItemLayout
        {
            IDiagram diagram;
            double firstX;
            int index;
            DiagramVisualItem lastEndpoint;
            double maxHeight;
            Dictionary<EndpointItem, int> position = new Dictionary<EndpointItem, int>();

            public EndpointItemLayout(IDiagram diagram)
            {
                this.diagram = diagram;
                SetMaxHeight();
            }

            public double MaxHeight
            {
                get { return maxHeight; }
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
                    endpointVisual.X = firstX + ((lastEndpoint.ActualWidth)*index);
                    endpointVisual.Y = lastEndpoint.Y;
                }

                if (endpointVisual.ActualHeight < MaxHeight)
                {
                    endpointVisual.Height = MaxHeight;
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

            void SetMaxHeight()
            {
                var endpoints = diagram.DiagramItems.OfType<EndpointItem>().ToList();
                maxHeight = endpoints.Select(endpoint => diagram.GetItemFromContainer(endpoint))
                    .Max(endpointVisual => endpointVisual.ActualHeight);
            }
        }
    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }
}