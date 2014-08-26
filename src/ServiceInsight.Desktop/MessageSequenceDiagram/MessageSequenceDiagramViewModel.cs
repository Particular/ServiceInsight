namespace Particular.ServiceInsight.Desktop.MessageSequenceDiagram
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Core.Settings;
    using Core.UI.ScreenManager;
    using Events;
    using Explorer.EndpointExplorer;
    using Framework;
    using MessageList;
    using Mindscape.WpfDiagramming;
    using Search;
    using ServiceControl;
    using Settings;


    public class MessageSequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IClipboard clipboard;
        SearchBarViewModel searchBar;
        MessageListViewModel messageList;
        ScreenFactory screenFactory;
        DefaultServiceControl serviceControl;
        IEventAggregator eventAggregator;
        IWindowManagerEx windowManager;
        ISettingsProvider settingsProvider;
        //ConcurrentDictionary<string, MessageNode> nodeMap;
        IMessageSequenceDiagramView view;
        string originalSelectionId = string.Empty;
        bool loadingConversation;
        EndpointExplorerViewModel endpointExplorer;

        public MessageSequenceDiagramViewModel(
            DefaultServiceControl serviceControl,
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            ScreenFactory screenFactory,
            SearchBarViewModel searchBar,
            MessageListViewModel messageList,
            ISettingsProvider settingsProvider,
            EndpointExplorerViewModel endpointExplorer,
            IClipboard clipboard)
        {
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            this.clipboard = clipboard;
            this.windowManager = windowManager;
            this.screenFactory = screenFactory;
            this.searchBar = searchBar;
            this.settingsProvider = settingsProvider;
            this.messageList = messageList;
            this.endpointExplorer = endpointExplorer;
            
            //Diagram = new MessageSequenceDiagram();
            //nodeMap = new ConcurrentDictionary<string, MessageNode>();
            ShowSequenceDiagram();
        }


        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = (IMessageSequenceDiagramView)view;
            //this.view.ShowMessage += OnShowMessage;
        }
        

        public MessageSequenceDiagram Diagram
        {
            get; set;
        }

        private void ShowSequenceDiagram()
        {
            Diagram = new MessageSequenceDiagram();

            // eCommerce Role
            var eCommerceNode = new RoleNode();
            eCommerceNode.Data = new { Name = "eCommerce" };
            Diagram.Nodes.Add(eCommerceNode);

            var eCommerceEndNode = new RoleEndNode();
            Diagram.Nodes.Add(eCommerceEndNode);

            // add start of the sequence
            var startSequenceNode = new SequenceStartNode();
            Diagram.Nodes.Add(startSequenceNode);

            var failedSequenceNode = new FailureNode();
            Diagram.Nodes.Add(failedSequenceNode);

            AddLifelineConnection(eCommerceNode, startSequenceNode);

            // Add activity node to eCommerce Lifeline
            var activityeCommerce = new ActivityNode();
            Diagram.Nodes.Add(activityeCommerce);
            AddLifelineConnection(startSequenceNode, activityeCommerce);
            AddLifelineConnection(activityeCommerce, failedSequenceNode);
            AddLifelineConnection(failedSequenceNode, eCommerceEndNode);


            // Sales Role
            var salesNode = new RoleNode();
            salesNode.Data = new { Name = "Sales" };
            Diagram.Nodes.Add(salesNode);

            var salesEndNode = new RoleEndNode();
            Diagram.Nodes.Add(salesEndNode);

            // Add activitiy node to the Sales Lifeline
            var activitySales1 = new ActivityNode();
            Diagram.Nodes.Add(activitySales1);
            AddLifelineConnection(salesNode, activitySales1);

            var activitySales2 = new ActivityNode();
            Diagram.Nodes.Add(activitySales2);
            AddTimeoutConnection(activitySales1, activitySales2);
            AddLifelineConnection(activitySales2, salesEndNode);


            // CustomerRelations
            var customerRelationsNode = new RoleNode();
            customerRelationsNode.Data = new { Name = "Customer Relations" };
            Diagram.Nodes.Add(customerRelationsNode);

            var customerRelationsEndNode = new RoleEndNode();
            Diagram.Nodes.Add(customerRelationsEndNode);
            AddLifelineConnection(customerRelationsNode, customerRelationsEndNode);


            // ContentManagement
            var contentManagementNode = new RoleNode();
            contentManagementNode.Data = new { Name = "Content Management" };
            Diagram.Nodes.Add(contentManagementNode);

            var contentManagementEndNode = new RoleEndNode();
            Diagram.Nodes.Add(contentManagementEndNode);
            AddLifelineConnection(contentManagementNode, contentManagementEndNode);


            // Operations
            var operationsNode = new RoleNode();
            operationsNode.Data = new { Name = "Operations" };
            Diagram.Nodes.Add(operationsNode);

            var operationsEndNode = new RoleEndNode();
            Diagram.Nodes.Add(operationsEndNode);
            AddLifelineConnection(operationsNode, operationsEndNode);


            // add connections
            AddMessageConnection(startSequenceNode, activitySales1, new { Name = "SubmitOrder", Type = "Message" });
            AddEventConnection(activitySales1, activityeCommerce, new { Name = "OrderPlaced", Type = "Event" });

            // Layout it
            UpdateLayout();
        }

        private DiagramConnection AddLifelineConnection(DiagramNode startNode, DiagramNode endNode)
        {
            var fromPoint = new DiagramConnectionPoint(startNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(endNode, Edge.Top);

            startNode.ConnectionPoints.Add(fromPoint);
            endNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection = new RoleLifelineConnection(fromPoint, toPoint);

            Diagram.Connections.Add(connection);
            return connection;
        }

        private DiagramConnection AddTimeoutConnection(ActivityNode startNode, ActivityNode endNode)
        {
            var fromPoint = new DiagramConnectionPoint(startNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(endNode, Edge.Top);

            startNode.ConnectionPoints.Add(fromPoint);
            endNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection = new TimeoutConnection(fromPoint, toPoint);

            Diagram.Connections.Add(connection);
            return connection;
        }

        private DiagramConnection AddEventConnection(DiagramNode startNode, DiagramNode endNode, object data)
        {
            var fromPoint = new DiagramConnectionPoint(startNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(endNode, Edge.Top);

            startNode.ConnectionPoints.Add(fromPoint);
            endNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection = new EventConnection(fromPoint, toPoint);
            connection.Data = data;

            Diagram.Connections.Add(connection);
            return connection;
        }

        private DiagramConnection AddMessageConnection(DiagramNode startNode, DiagramNode endNode, object data)
        {
            var fromPoint = startNode.GetType() == typeof(SequenceStartNode) ? 
                new DiagramConnectionPoint(startNode, Edge.Right) :
                new DiagramConnectionPoint(startNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(endNode, Edge.Top);

            startNode.ConnectionPoints.Add(fromPoint);
            endNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection = new MessageConnection(fromPoint, toPoint);
            connection.Data = data;

            Diagram.Connections.Add(connection);
            return connection;
        }




        //public MessageNode SelectedMessage { get; set; }
        public bool ShowEndpoints { get; set; }

        public void ZoomIn()
        {
            view.Surface.Zoom += 0.1;
        }

        public void ZoomOut()
        {
            view.Surface.Zoom -= 0.1;
        }

        public async void Handle(SelectedMessageChanged @event)
        {
            if (loadingConversation) return;

            loadingConversation = true;
            //nodeMap.Clear();
            //Diagram = new MessageSequenceDiagram();
            ShowSequenceDiagram();

            var storedMessage = @event.Message;
            if (storedMessage == null)
            {
                loadingConversation = false;
                return;
            }

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
            {
                loadingConversation = false;
                return;
            }

            try
            {
                //var relatedMessagesTask = await serviceControl.GetConversationById(conversationId);
                //var nodes = relatedMessagesTask.ConvertAll(CreateMessageNode);

                //UpdateLayout();
            }
            finally
            {
                loadingConversation = false;
            }
        }

        private void UpdateLayout()
        {
            if (view != null)
            {
                view.ApplyLayout();
                view.SizeToFit();
            }
        }

    }
}
