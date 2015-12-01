namespace ServiceInsight.DiagramLegend
{
    using System.Collections.ObjectModel;
    using Caliburn.Micro;
    using SequenceDiagram.Diagram;
    using Particular.ServiceInsight.Desktop.Models;

    public class DiagramLegendViewModel : Screen
    {
        private const int ArrowWidth = 100;
        private const int LocalArrowWidth = 30;
        private const int HandlerWidth = 20;
        private const int HandlerHeight = 50;
        
        public DiagramLegendViewModel()
        {
            DiagramItemsDescription = new BindableCollection<DiagramItemDescription>();
            GenerateLegendData();
        }

        public ObservableCollection<DiagramItemDescription> DiagramItemsDescription { get; }

        void GenerateLegendData()
        {
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateConversationStartMessage(), "Start of a Conversation"));
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateHandler(), "Handling and processing of a Message"));
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateErrorHandler(), "Handling of a Message failed"));
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateEventMessage(), "Publishing an Event"));
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateCommandMessage(), "Sending a Command"));
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateLocalSendCommandMessage(), "Sending a Local Message"));
            DiagramItemsDescription.Add(new DiagramItemDescription(CreateTimeoutMessage(), "Raising a Timeout"));
            //DiagramItemsDescription.Add(new DiagramItemDescription(CreateMessageRoute(), "Connects Message and Handler"));
        }

        private DiagramVisualItem CreateMessageRoute()
        {
            var route = new MessageProcessingRoute(null, null);
            var diagramItem = new DiagramVisualItem
            {
                Content = route,
                Height = 100,
                Width = 20
            };

            return diagramItem;
        }

        DiagramVisualItem CreateErrorHandler()
        {
            var handler = new Handler("Failed", null) { State = HandlerState.Fail };
            var diagramItem = new DiagramVisualItem
            {
                Content = handler,
                Height = HandlerHeight,
                Width = HandlerWidth
            };

            return diagramItem;
        }

        DiagramVisualItem CreateConversationStartMessage()
        {
            var handler = new Handler("First", null) { State = HandlerState.Success };
            var diagramItem = new DiagramVisualItem
            {
                Content = handler,
                Height = HandlerHeight,
                Width = HandlerWidth
            };

            return diagramItem;
        }

        DiagramVisualItem CreateTimeoutMessage()
        {
            var arrow = new Arrow("Timeout", null, MessageStatus.Successful, "Timeout", null, null, null)
            {
                Width = LocalArrowWidth,
                Type = ArrowType.Timeout
            };

            return new DiagramVisualItem { Content = arrow };
        }

        DiagramVisualItem CreateLocalSendCommandMessage()
        {
            var arrow = new Arrow("LocalSend", null, MessageStatus.Successful, "LocalSend", null, null, null)
            {
                Width = LocalArrowWidth,
                Type = ArrowType.Local
            };

            return new DiagramVisualItem { Content = arrow };
        }

        DiagramVisualItem CreateCommandMessage()
        {
            var arrow = new Arrow("Command", null, MessageStatus.Successful, "Command", null, null, null)
            {
                Width = ArrowWidth,
                Type = ArrowType.Command
            };

            return new DiagramVisualItem { Content = arrow };
        }

        DiagramVisualItem CreateEventMessage()
        {
            var arrow = new Arrow("Event", null, MessageStatus.Successful, "Event", null, null, null)
            {
                Width = 100,
                Type = ArrowType.Event
            };

            var diagramItem = new DiagramVisualItem
            {
                Content = arrow,
            };

            return diagramItem;
        }

        private DiagramVisualItem CreateHandler()
        {
            var handler = new Handler("Handler", null) { State = HandlerState.Success };
            var diagramItem = new DiagramVisualItem
            {
                Content = handler,
                Height = HandlerHeight,
                Width = HandlerWidth
            };

            return diagramItem;
        }
    }

    public class DiagramItemDescription
    {
        public DiagramItemDescription(DiagramVisualItem item, string description)
        {
            Item = item;
            Description = description;
        }

        public DiagramVisualItem Item { get; }
        public string Description { get; }
    }
}