﻿namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using ServiceInsight.ExtensionMethods;

    public class SequenceDiagramLayoutManager : ILayoutManager
    {
        public void PerformLayout(IDiagram diagram)
        {
            if (diagram.DiagramItems == null || diagram.DiagramItems.Count == 0)
            {
                return;
            }

            diagram.Padding = default;

            var endpointLayout = new EndpointItemLayout(diagram);
            var handlerLayout = new HandlerLayout(diagram);
            var endpointTimelineLayout = new EndpointTimelineLayout(diagram);
            var arrowLayout = new ArrowLayout(diagram, endpointLayout);
            var processRouteLayout = new ProcessRouteLayout(diagram);

            foreach (var item in diagram.DiagramItems)
            {
                if (item is EndpointItem endpoint)
                {
                    endpointLayout.Position(endpoint);
                    continue;
                }

                if (item is Handler handler)
                {
                    handlerLayout.Position(handler);
                    continue;
                }

                if (item is EndpointTimeline timeline)
                {
                    endpointTimelineLayout.Position(timeline);
                    continue;
                }

                if (item is Arrow arrow)
                {
                    arrowLayout.Position(arrow);
                }

                if (item is MessageProcessingRoute route)
                {
                    processRouteLayout.Position(route);
                }
            }
        }

        class ProcessRouteLayout
        {
            readonly IDiagram diagram;
            const double ArrowBoundary = 8;
            const double ArrowHeadHeight = 5;

            public ProcessRouteLayout(IDiagram diagram)
            {
                this.diagram = diagram;
            }

            public void Position(MessageProcessingRoute route)
            {
                var handler = diagram.GetItemFromContainer(route.ProcessingHandler);
                if (handler == null)
                {
                    return;
                }

                var arrow = diagram.GetItemFromContainer(route.FromArrow);
                if (arrow == null)
                {
                    return;
                }

                var routeVisual = diagram.GetItemFromContainer(route);
                if (routeVisual == null)
                {
                    return;
                }

                var height = Math.Abs(arrow.Y - handler.Y);

                routeVisual.X = handler.X + ((handler.ActualWidth - routeVisual.ActualWidth) / 2);
                routeVisual.Y = arrow.Y + ArrowBoundary + ArrowHeadHeight;
                routeVisual.Height = height - (3 * ArrowHeadHeight) + 1;
            }
        }

        class ArrowLayout
        {
            const double ArrowHeadWidth = 4;

            readonly IDiagram diagram;
            readonly EndpointItemLayout endpointItemLayout;

            public ArrowLayout(IDiagram diagram, EndpointItemLayout endpointItemLayout)
            {
                this.diagram = diagram;
                this.endpointItemLayout = endpointItemLayout;
            }

            public void Position(Arrow arrow)
            {
                var arrowVisual = diagram.GetItemFromContainer(arrow);
                if (arrowVisual == null)
                {
                    return;
                }

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

                var fromHandlerVisual = diagram.GetItemFromContainer(fromHandler);
                if (fromHandlerVisual == null)
                {
                    return;
                }

                var toHandlerVisual = diagram.GetItemFromContainer(arrow.ToHandler);
                if (toHandlerVisual == null)
                {
                    return;
                }

                var messageProcessingRouteVisual = diagram.GetItemFromContainer(arrow.MessageProcessingRoute);
                if (messageProcessingRouteVisual == null)
                {
                    return;
                }

                arrowVisual.HilightChangedEvent += (sender, e) =>
                {
                    toHandlerVisual.InternalSetHilight((bool)e.NewValue);
                    messageProcessingRouteVisual.InternalSetHilight((bool)e.NewValue);
                    messageProcessingRouteVisual.ZIndex = messageProcessingRouteVisual.Hilight ? 10 : 0;
                };

                toHandlerVisual.HilightChangedEvent += (sender, e) =>
                {
                    arrowVisual.InternalSetHilight((bool)e.NewValue);
                };

                arrowVisual.X = fromHandlerVisual.X;
                var arrowIndex = fromHandler.Out.IndexOf(arrow) + 1;
#pragma warning disable IDE0047 // Remove unnecessary parentheses (false positive)
                arrowVisual.Y = fromHandlerVisual.Y + ((fromHandlerVisual.Height / (fromHandler.Out.Count() + 1)) * arrowIndex) - 15;
#pragma warning restore IDE0047 // Remove unnecessary parentheses (false positive)

                if (fromEndpointIndex == toEndpointIndex)
                {
                    //Local
                    arrow.Direction = Direction.Right;
                    arrowVisual.X += fromHandlerVisual.ActualWidth;
                    arrow.Width = 15 + ArrowHeadWidth; //fixed, small size for local sends and timeouts
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
                    arrow.Width = fromHandlerVisual.X - (toHandlerVisual.X + toHandlerVisual.ActualWidth) - ArrowHeadWidth;
                    arrowVisual.X = fromHandlerVisual.X - arrowVisual.ActualWidth;

                    if (arrowVisual.X < 0)
                    {
                        diagram.Padding = new Thickness(Math.Max(diagram.Padding.Left, Math.Abs(Math.Floor(arrowVisual.X))), 0, 0, 0);
                    }
                }

                // The handler needs to refresh based on the direction of the arrow.
                fromHandler.Refresh();
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
                if (timelineVisual == null)
                {
                    return;
                }

                var endpointVisual = diagram.GetItemFromContainer(timeline.Endpoint);
                if (endpointVisual == null)
                {
                    return;
                }

                timelineVisual.X = endpointVisual.X + (endpointVisual.ActualWidth / 2);
                timelineVisual.Y = endpointVisual.Y + endpointVisual.ActualHeight;
                timelineVisual.Height = maxHeight;
            }

            double GetMaxHeight()
            {
                var handlers = diagram.DiagramItems.OfType<Handler>().ToList();
                var handlerVisuals = handlers.Select(handler => diagram.GetItemFromContainer(handler))
                                             .Where(h => h != null)
                                             .ToList();

                var height = handlerVisuals.Select(h => h.Y)
                                           .DefaultIfEmpty()
                                           .Max();

                return height + 50; //Continue a bit from the last handler
            }
        }

        class HandlerLayout
        {
            IDiagram diagram;
            double nextY;

            public HandlerLayout(IDiagram diagram)
            {
                this.diagram = diagram;
                nextY = 25;
            }

            public void Position(Handler handler)
            {
                var handlerVisual = diagram.GetItemFromContainer(handler);
                if (handlerVisual == null)
                {
                    return;
                }

                var endpointVisual = diagram.GetItemFromContainer(handler.Endpoint);
                if (endpointVisual == null)
                {
                    return;
                }

                handlerVisual.X = endpointVisual.X + (endpointVisual.ActualWidth / 2) - 7;

                var count = handler.Out.Count();
                var height = (count == 0 ? 1 : count) * 40;
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
            Dictionary<EndpointItem, int> position = new Dictionary<EndpointItem, int>();

            public EndpointItemLayout(IDiagram diagram)
            {
                this.diagram = diagram;
                SetMaxHeight();
            }

            public double MaxHeight { get; private set; }

            public void Position(EndpointItem endpoint)
            {
                var endpointVisual = diagram.GetItemFromContainer(endpoint);
                if (endpointVisual == null)
                {
                    return;
                }

                if (index == 0)
                {
                    firstX = endpointVisual.X;
                }

                if (lastEndpoint != null)
                {
                    endpointVisual.X = firstX + (lastEndpoint.ActualWidth * index);
                }

                if (endpointVisual.ActualHeight < MaxHeight)
                {
                    endpointVisual.Height = MaxHeight;
                }

                endpointVisual.Y = -MaxHeight;

                lastEndpoint = endpointVisual;
                position[endpoint] = index;
                index++;
            }

            public int GetIndexPosition(EndpointItem endpoint) => position[endpoint];

            public EndpointItem GetFirst() => position.Single(kv => kv.Value == 0).Key;

            void SetMaxHeight()
            {
                var endpoints = diagram.DiagramItems.OfType<EndpointItem>().ToList();
                var visualItems = endpoints.Select(endpoint => diagram.GetItemFromContainer(endpoint))
                                     .Where(e => e != null)
                                     .ToList();

                MaxHeight = visualItems.Select(endpointVisual => endpointVisual.ActualHeight)
                                       .DefaultIfEmpty()
                                       .Max();
            }
        }
    }

    public interface ILayoutManager
    {
        void PerformLayout(IDiagram diagram);
    }
}