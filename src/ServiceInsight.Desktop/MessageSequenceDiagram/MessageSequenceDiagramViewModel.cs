using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Core.Settings;
using NServiceBus.Profiler.Desktop.Events;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.MessageList;
using Mindscape.WpfDiagramming;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.ServiceControl;

namespace NServiceBus.Profiler.Desktop.MessageSequenceDiagram
{
    public interface IMessageSequenceDiagramViewModel : IScreen,
    IHandle<SelectedMessageChanged>
    {
        MessageSequenceDiagram Diagram { get; }
        //MessageNode SelectedMessage { get; set; }
        bool ShowEndpoints { get; set; }
        //void CopyMessageUri(StoredMessage message);
        //void CopyConversationId(StoredMessage message);
        //void SearchByMessageId(StoredMessage message);
        //Task RetryMessage(StoredMessage message);
        //void ShowMessageBody(StoredMessage message);
        //void ShowSagaWindow(StoredMessage message);
        //void ToggleEndpointData();
        //void ShowException(IExceptionDetails exception);
        void ZoomIn();
        void ZoomOut();
        //bool IsFocused(MessageInfo message);
    }


    public class MessageSequenceDiagramViewModel : Screen, IMessageSequenceDiagramViewModel
    {
        private readonly ISearchBarViewModel _searchBar;
        private readonly IMessageListViewModel _messageList;
        private readonly IScreenFactory _screenFactory;
        private readonly IServiceControl _serviceControl;
        private readonly IEventAggregator _eventAggregator;
        private readonly IClipboard _clipboard;
        private readonly IWindowManagerEx _windowManager;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IEndpointExplorerViewModel _endpointExplorer;
        private IMessageSequenceDiagramView _view;
        private bool _loadingConversation;
        //private readonly ConcurrentDictionary<string, MessageNode> _nodeMap;

        public MessageSequenceDiagramViewModel(
            IServiceControl serviceControl,
            IEventAggregator eventAggregator,
            IClipboard clipboard,
            IWindowManagerEx windowManager,
            IScreenFactory screenFactory,
            ISearchBarViewModel searchBar,
            IMessageListViewModel messageList,
            ISettingsProvider settingsProvider,
            IEndpointExplorerViewModel endpointExplorer)
        {
            _serviceControl = serviceControl;
            _eventAggregator = eventAggregator;
            _clipboard = clipboard;
            _windowManager = windowManager;
            _screenFactory = screenFactory;
            _searchBar = searchBar;
            _settingsProvider = settingsProvider;
            _messageList = messageList;
            _endpointExplorer = endpointExplorer;
            
            Diagram = new MessageSequenceDiagram();
            //_nodeMap = new ConcurrentDictionary<string, MessageNode>();
            ShowSequenceDiagram();
        }


        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IMessageSequenceDiagramView)view;
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

            var eCommerceEndNode = new SDFailureNode();
            Diagram.Nodes.Add(eCommerceEndNode);

            // add start of the sequence
            var startSequenceNode = new SequenceStartNode();
            Diagram.Nodes.Add(startSequenceNode);

            var endSequenceNode = new SequenceEndNode();
            Diagram.Nodes.Add(endSequenceNode);

            AddLifelineConnection(eCommerceNode, startSequenceNode);

            // Add activity node to eCommerce Lifeline
            var activityeCommerce = new ActivityNode();
            Diagram.Nodes.Add(activityeCommerce);
            AddLifelineConnection(startSequenceNode, activityeCommerce);
            AddLifelineConnection(activityeCommerce, endSequenceNode);
            AddLifelineConnection(endSequenceNode, eCommerceEndNode);


            // Sales Role
            var salesNode = new RoleNode();
            salesNode.Data = new { Name = "Sales" };
            Diagram.Nodes.Add(salesNode);

            var salesEndNode = new SDFailureNode();
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

            var customerRelationsEndNode = new SDFailureNode();
            Diagram.Nodes.Add(customerRelationsEndNode);
            AddLifelineConnection(customerRelationsNode, customerRelationsEndNode);


            // ContentManagement
            var contentManagementNode = new RoleNode();
            contentManagementNode.Data = new { Name = "Content Management" };
            Diagram.Nodes.Add(contentManagementNode);

            var contentManagementEndNode = new SDFailureNode();
            Diagram.Nodes.Add(contentManagementEndNode);
            AddLifelineConnection(contentManagementNode, contentManagementEndNode);


            // Operations
            var operationsNode = new RoleNode();
            operationsNode.Data = new { Name = "Operations" };
            Diagram.Nodes.Add(operationsNode);

            var operationsEndNode = new SDFailureNode();
            Diagram.Nodes.Add(operationsEndNode);
            AddLifelineConnection(operationsNode, operationsEndNode);


            // add connections
            //AddSDEventConnection(startSequenceNode, activitySales1, new { Name = "SubmitOrder" });
            //AddSDEventConnection(activitySales1, activityeCommerce, new { Name = "OrderPlaced" });

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

        private DiagramConnection AddSDEventConnection(DiagramNode startNode, DiagramNode endNode, object data)
        {
            var fromPoint = new DiagramConnectionPoint(startNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(endNode, Edge.Top);

            startNode.ConnectionPoints.Add(fromPoint);
            endNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection = new SDEventConnection(fromPoint, toPoint);
            connection.Data = data;

            Diagram.Connections.Add(connection);
            return connection;
        }

        private DiagramConnection AddSDMessageConnection(DiagramNode startNode, DiagramNode endNode, object data)
        {
            var fromPoint = new DiagramConnectionPoint(startNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(endNode, Edge.Top);

            startNode.ConnectionPoints.Add(fromPoint);
            endNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection = new SDMessageConnection(fromPoint, toPoint);
            connection.Data = data;

            Diagram.Connections.Add(connection);
            return connection;
        }




        //public MessageNode SelectedMessage { get; set; }
        public bool ShowEndpoints { get; set; }

        public void ZoomIn()
        {
            _view.Surface.Zoom += 0.1;
        }

        public void ZoomOut()
        {
            _view.Surface.Zoom -= 0.1;
        }

        public async void Handle(SelectedMessageChanged @event)
        {
            if (_loadingConversation) return;

            _loadingConversation = true;
            //_nodeMap.Clear();
            //Diagram = new MessageSequenceDiagram();

            var storedMessage = @event.Message;
            if (storedMessage == null)
            {
                _loadingConversation = false;
                return;
            }

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
            {
                _loadingConversation = false;
                return;
            }

            try
            {
                //var relatedMessagesTask = await _serviceControl.GetConversationById(conversationId);
                //var nodes = relatedMessagesTask.ConvertAll(CreateMessageNode);

                //UpdateLayout();
            }
            finally
            {
                _loadingConversation = false;
            }
        }

        private void UpdateLayout()
        {
            if (_view != null)
            {
                _view.ApplyLayout();
                _view.SizeToFit();
            }
        }

    }
}
