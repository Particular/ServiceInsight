namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Core.Settings;
    using Core.UI.ScreenManager;
    using Events;
    using ExceptionHandler;
    using Explorer.EndpointExplorer;
    using MessageList;
    using Mindscape.WpfDiagramming;
    using Models;
    using Search;
    using ServiceControl;
    using Settings;

    public interface IMessageFlowViewModel : IScreen, 
        IHandle<SelectedMessageChanged>
    {
        MessageFlowDiagram Diagram { get; }
        MessageNode SelectedMessage { get; set; }
        bool ShowEndpoints { get; set; }
        void CopyMessageUri(StoredMessage message);
        void CopyConversationId(StoredMessage message);
        void SearchByMessageId(StoredMessage message);
        Task RetryMessage(StoredMessage message);
        void ShowMessageBody(StoredMessage message);
        void ShowSagaWindow(StoredMessage message);
        void ToggleEndpointData();
        void ShowException(IExceptionDetails exception);
        void ZoomIn();
        void ZoomOut();
        bool IsFocused(MessageInfo message);
    }

    public class MessageFlowViewModel : Screen, IMessageFlowViewModel
    {
        private readonly ISearchBarViewModel _searchBar;
        private readonly IMessageListViewModel _messageList;
        private readonly IScreenFactory _screenFactory;
        private readonly IServiceControl _serviceControl;
        private readonly IEventAggregator _eventAggregator;
        private readonly IClipboard _clipboard;
        private readonly IWindowManagerEx _windowManager;
        private readonly ISettingsProvider _settingsProvider;
        private readonly ConcurrentDictionary<string, MessageNode> _nodeMap;
        private IMessageFlowView _view;
        private string _originalSelectionId = string.Empty;
        private bool _loadingConversation;
        private IEndpointExplorerViewModel _endpointExplorer;

        public MessageFlowViewModel(
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

            Diagram = new MessageFlowDiagram();
            _nodeMap = new ConcurrentDictionary<string, MessageNode>();
        }

        public MessageFlowDiagram Diagram
        {
            get; set;
        }

        public bool ShowEndpoints
        {
            get; set;
        }

        public MessageNode SelectedMessage
        {
            get; set;
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IMessageFlowView)view;
            _view.ShowMessage += OnShowMessage;
        }

        private void OnShowMessage(object sender, SearchMessageEventArgs e)
        {
            SearchByMessageId(e.MessageNode.Message);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var settings = _settingsProvider.GetSettings<ProfilerSettings>();
            
            ShowEndpoints = settings.ShowEndpoints;
        }

        public void ShowMessageBody(StoredMessage message)
        {
            _eventAggregator.Publish(new SwitchToMessageBody());
        }

        public void ShowSagaWindow()
        {
            if (!_messageList.Rows.Any(r => r.Id == SelectedMessage.Message.Id))
            {
                _endpointExplorer.SelectedNode = _endpointExplorer.ServiceControlRoot;
            }
            _messageList.Focus(SelectedMessage.Message);
            _eventAggregator.Publish(new SwitchToSagaWindow());
        }

        public void ShowSagaWindow(StoredMessage message)
        {
            ShowSagaWindow();
        }

        public void ShowException(IExceptionDetails exception)
        {
            var model = _screenFactory.CreateScreen<IExceptionDetailViewModel>();
            model.Exception = exception;
            _windowManager.ShowDialog(model, true);
        }

        public void ToggleEndpointData()
        {
            ShowEndpoints = !ShowEndpoints;
        }

        public void CopyConversationId(StoredMessage message)
        {
            _clipboard.CopyTo(message.ConversationId);
        }

        public void CopyMessageUri(StoredMessage message)
        {
            _clipboard.CopyTo(_serviceControl.GetUri(message).ToString());
        }

        public void SearchByMessageId(StoredMessage message)
        {
            _searchBar.Search(performSearch: false, searchQuery: message.MessageId);
            _eventAggregator.Publish(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }

        public async Task RetryMessage(StoredMessage message)
        {
            _eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", message.SendingEndpoint));
            await _serviceControl.RetryMessage(message.Id);
            _eventAggregator.Publish(new MessageStatusChanged(message.MessageId, MessageStatus.RetryIssued));
            _eventAggregator.Publish(new WorkFinished());
        }

        public async void Handle(SelectedMessageChanged @event)
        {
            if (_loadingConversation) return;

            _loadingConversation = true;
            _originalSelectionId = string.Empty;
            _nodeMap.Clear();

            SelectedMessage = null;
            Diagram = new MessageFlowDiagram();
        
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
                var relatedMessagesTask = await _serviceControl.GetConversationById(conversationId);
                var nodes = relatedMessagesTask.ConvertAll(CreateMessageNode);

                CreateConversationNodes(storedMessage.Id, nodes);
                LinkConversationNodes(nodes);
                UpdateLayout();
            }
            finally
            {
                _loadingConversation = false;
            }
        }

        public void ZoomIn()
        {
            _view.Surface.Zoom += 0.1;
        }

        public void ZoomOut()
        {
            _view.Surface.Zoom -= 0.1;
        }

        public bool IsFocused(MessageInfo message)
        {
            return message.Id == _originalSelectionId;
        }

        public void OnShowEndpointsChanged()
        {
            foreach (var node in Diagram.Nodes.OfType<MessageNode>())
            {
                node.ShowEndpoints = ShowEndpoints;
                if(_view != null) _view.UpdateNode(node);
            }

            if (_view != null)
            {
                _view.UpdateConnections();
                _view.ApplyLayout();
            }

            UpdateSetting();
        }

        private void UpdateSetting()
        {
            var settings = _settingsProvider.GetSettings<ProfilerSettings>();
            if (settings.ShowEndpoints != ShowEndpoints)
            {
                settings.ShowEndpoints = ShowEndpoints;
                _settingsProvider.SaveSettings(settings);
            }
        }

        private void LinkConversationNodes(IEnumerable<MessageNode> relatedMessagesTask)
        {
            foreach (var msg in relatedMessagesTask)
            {
                if (msg.Message.RelatedToMessageId == null &&
                    msg.Message.RelatedToMessageId != msg.Message.MessageId)
                {
                    continue;
                }

                var parentMessage = _nodeMap.Values.SingleOrDefault(m => 
                    m.Message != null && m.Message.ReceivingEndpoint != null && m.Message.SendingEndpoint != null &&
                    m.Message.MessageId == msg.Message.RelatedToMessageId && 
                    m.Message.ReceivingEndpoint.Name == msg.Message.SendingEndpoint.Name);

                if (parentMessage == null)
                    continue;

                AddConnection(parentMessage, msg);
            }
        }

        private void AddConnection(MessageNode parentNode, MessageNode childNode)
        {
            var fromPoint = new DiagramConnectionPoint(parentNode, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(childNode, Edge.Top);
            
            parentNode.ConnectionPoints.Add(fromPoint);
            childNode.ConnectionPoints.Add(toPoint);

            DiagramConnection connection;

            if (childNode.IsPublished)
            {
                connection = new EventConnection(fromPoint, toPoint);
            }
            else
            {
                connection = new CommandConnection(fromPoint, toPoint);
            }

            Diagram.Connections.Add(connection);
        }

        private void CreateConversationNodes(string selectedId, IEnumerable<MessageNode> relatedNodes)
        {
            foreach (var node in relatedNodes)
            {
                if (string.Equals(node.Message.Id, selectedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    _originalSelectionId = selectedId;
                    SelectedMessage = node;
                }

                _nodeMap.TryAdd(node.Message.Id, node);
                Diagram.Nodes.Add(node);
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

        private MessageNode CreateMessageNode(StoredMessage x)
        {
            return new MessageNode(this, x) { ShowEndpoints = ShowEndpoints };
        }
    }
}