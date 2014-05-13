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
        void ShowException(ExceptionDetails exception);
        void ZoomIn();
        void ZoomOut();
        bool IsFocused(MessageInfo message);
    }

    public class MessageFlowViewModel : Screen, IMessageFlowViewModel
    {
        ISearchBarViewModel searchBar;
        IMessageListViewModel messageList;
        ScreenFactory screenFactory;
        DefaultServiceControl serviceControl;
        IEventAggregator eventAggregator;
        IClipboard clipboard;
        IWindowManagerEx windowManager;
        ISettingsProvider settingsProvider;
        ConcurrentDictionary<string, MessageNode> nodeMap;
        IMessageFlowView view;
        string originalSelectionId = string.Empty;
        bool loadingConversation;
        IEndpointExplorerViewModel endpointExplorer;

        public MessageFlowViewModel(
            DefaultServiceControl serviceControl,
            IEventAggregator eventAggregator,
            IClipboard clipboard, 
            IWindowManagerEx windowManager,
            ScreenFactory screenFactory,
            ISearchBarViewModel searchBar, 
            IMessageListViewModel messageList,
            ISettingsProvider settingsProvider,
            IEndpointExplorerViewModel endpointExplorer)
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

            Diagram = new MessageFlowDiagram();
            nodeMap = new ConcurrentDictionary<string, MessageNode>();
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
            this.view = (IMessageFlowView)view;
            this.view.ShowMessage += OnShowMessage;
        }

        void OnShowMessage(object sender, SearchMessageEventArgs e)
        {
            SearchByMessageId(e.MessageNode.Message);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var settings = settingsProvider.GetSettings<ProfilerSettings>();
            
            ShowEndpoints = settings.ShowEndpoints;
        }

        public void ShowMessageBody(StoredMessage message)
        {
            eventAggregator.Publish(new SwitchToMessageBody());
        }

        public void ShowSagaWindow()
        {
            if (messageList.Rows.All(r => r.Id != SelectedMessage.Message.Id))
            {
                endpointExplorer.SelectedNode = endpointExplorer.ServiceControlRoot;
            }
            messageList.Focus(SelectedMessage.Message);
            eventAggregator.Publish(new SwitchToSagaWindow());
        }

        public void ShowSagaWindow(StoredMessage message)
        {
            ShowSagaWindow();
        }

        public void ShowException(ExceptionDetails exception)
        {
            var model = screenFactory.CreateScreen<IExceptionDetailViewModel>();
            model.Exception = exception;
            windowManager.ShowDialog(model, true);
        }

        public void ToggleEndpointData()
        {
            ShowEndpoints = !ShowEndpoints;
        }

        public void CopyConversationId(StoredMessage message)
        {
            clipboard.CopyTo(message.ConversationId);
        }

        public void CopyMessageUri(StoredMessage message)
        {
            clipboard.CopyTo(serviceControl.GetUri(message).ToString());
        }

        public void SearchByMessageId(StoredMessage message)
        {
            searchBar.Search(performSearch: false, searchQuery: message.MessageId);
            eventAggregator.Publish(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }

        public async Task RetryMessage(StoredMessage message)
        {
            eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", message.SendingEndpoint));
            await serviceControl.RetryMessage(message.Id);
            eventAggregator.Publish(new RetryMessage{MessageId = message.MessageId});
            eventAggregator.Publish(new WorkFinished());
        }

        public async void Handle(SelectedMessageChanged @event)
        {
            if (loadingConversation) return;

            loadingConversation = true;
            originalSelectionId = string.Empty;
            nodeMap.Clear();

            SelectedMessage = null;
            Diagram = new MessageFlowDiagram();
        
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
                var relatedMessagesTask = await serviceControl.GetConversationById(conversationId);
                var nodes = relatedMessagesTask.ConvertAll(CreateMessageNode);

                CreateConversationNodes(storedMessage.Id, nodes);
                LinkConversationNodes(nodes);
                UpdateLayout();
            }
            finally
            {
                loadingConversation = false;
            }
        }

        public void ZoomIn()
        {
            view.Surface.Zoom += 0.1;
        }

        public void ZoomOut()
        {
            view.Surface.Zoom -= 0.1;
        }

        public bool IsFocused(MessageInfo message)
        {
            return message.Id == originalSelectionId;
        }

        public void OnShowEndpointsChanged()
        {
            foreach (var node in Diagram.Nodes.OfType<MessageNode>())
            {
                node.ShowEndpoints = ShowEndpoints;
                if(view != null) view.UpdateNode(node);
            }

            if (view != null)
            {
                view.UpdateConnections();
                view.ApplyLayout();
            }

            UpdateSetting();
        }

        void UpdateSetting()
        {
            var settings = settingsProvider.GetSettings<ProfilerSettings>();
            if (settings.ShowEndpoints != ShowEndpoints)
            {
                settings.ShowEndpoints = ShowEndpoints;
                settingsProvider.SaveSettings(settings);
            }
        }

        void LinkConversationNodes(IEnumerable<MessageNode> relatedMessagesTask)
        {
            foreach (var msg in relatedMessagesTask)
            {
                if (msg.Message.RelatedToMessageId == null &&
                    msg.Message.RelatedToMessageId != msg.Message.MessageId)
                {
                    continue;
                }

                var parentMessage = nodeMap.Values.SingleOrDefault(m => 
                    m.Message != null && m.Message.ReceivingEndpoint != null && m.Message.SendingEndpoint != null &&
                    m.Message.MessageId == msg.Message.RelatedToMessageId && 
                    m.Message.ReceivingEndpoint.Name == msg.Message.SendingEndpoint.Name);

                if (parentMessage == null)
                    continue;

                AddConnection(parentMessage, msg);
            }
        }

        void AddConnection(MessageNode parentNode, MessageNode childNode)
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

        void CreateConversationNodes(string selectedId, IEnumerable<MessageNode> relatedNodes)
        {
            foreach (var node in relatedNodes)
            {
                if (string.Equals(node.Message.Id, selectedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    originalSelectionId = selectedId;
                    SelectedMessage = node;
                }

                nodeMap.TryAdd(node.Message.Id, node);
                Diagram.Nodes.Add(node);
            }
        }

        void UpdateLayout()
        {
            if (view != null)
            {
                view.ApplyLayout();
                view.SizeToFit();
            }
        }

        MessageNode CreateMessageNode(StoredMessage x)
        {
            return new MessageNode(this, x) { ShowEndpoints = ShowEndpoints };
        }
    }
}