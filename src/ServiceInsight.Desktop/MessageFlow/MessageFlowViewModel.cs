namespace Particular.ServiceInsight.Desktop.MessageFlow
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Common;
    using Core.Settings;
    using Core.UI.ScreenManager;
    using Events;
    using Explorer.EndpointExplorer;
    using Framework;
    using MessageList;
    using Mindscape.WpfDiagramming;
    using Models;
    using Search;
    using ServiceControl;
    using Settings;

    public class MessageFlowViewModel : Screen,
        IHandle<SelectedMessageChanged>
    {
        private readonly IClipboard clipboard;
        SearchBarViewModel searchBar;
        MessageListViewModel messageList;
        ScreenFactory screenFactory;
        IServiceControl serviceControl;
        IEventAggregator eventAggregator;
        IWindowManagerEx windowManager;
        ISettingsProvider settingsProvider;
        ConcurrentDictionary<string, MessageNode> nodeMap;
        IMessageFlowView view;
        string originalSelectionId = string.Empty;
        bool loadingConversation;
        EndpointExplorerViewModel endpointExplorer;

        public MessageFlowViewModel(
            IServiceControl serviceControl,
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            ScreenFactory screenFactory,
            SearchBarViewModel searchBar,
            MessageListViewModel messageList,
            ISettingsProvider settingsProvider,
            EndpointExplorerViewModel endpointExplorer,
            IClipboard clipboard)
        {
            this.clipboard = clipboard;
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            this.windowManager = windowManager;
            this.screenFactory = screenFactory;
            this.searchBar = searchBar;
            this.settingsProvider = settingsProvider;
            this.messageList = messageList;
            this.endpointExplorer = endpointExplorer;

            CopyConversationIDCommand = new CopyConversationIDCommand(clipboard);
            CopyMessageURICommand = new CopyMessageURICommand(clipboard, serviceControl);
            SearchByMessageIDCommand = new SearchByMessageIDCommand(eventAggregator, searchBar);
            RetryMessageCommand = new RetryMessageCommand(eventAggregator, serviceControl);

            Diagram = new MessageFlowDiagram();
            nodeMap = new ConcurrentDictionary<string, MessageNode>();
        }

        public MessageFlowDiagram Diagram
        {
            get;
            set;
        }

        public bool ShowEndpoints
        {
            get;
            set;
        }

        public MessageNode SelectedMessage
        {
            get;
            set;
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
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

        public void ShowMessageBody()
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

        public void ShowException(ExceptionDetails exception)
        {
            var model = screenFactory.CreateScreen<ExceptionDetailViewModel>();
            model.Exception = exception;
            windowManager.ShowDialog(model);
        }

        public void ToggleEndpointData()
        {
            ShowEndpoints = !ShowEndpoints;
        }

        public ICommand CopyConversationIDCommand { get; private set; }
        public ICommand CopyMessageURICommand { get; private set; }
        public ICommand SearchByMessageIDCommand { get; private set; }
        public ICommand RetryMessageCommand { get; private set; }

        public void SearchByMessageId(StoredMessage message, bool performSearch = false)
        {
            searchBar.Search(performSearch: performSearch, searchQuery: message.MessageId);
            eventAggregator.Publish(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }

        //public void RetryMessage(StoredMessage message)
        //{
        //    eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", message.SendingEndpoint));
        //    serviceControl.RetryMessage(message.Id);
        //    eventAggregator.Publish(new RetryMessage { MessageId = message.MessageId });
        //    eventAggregator.Publish(new WorkFinished());
        //}

        public void Handle(SelectedMessageChanged @event)
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
                var relatedMessagesTask = serviceControl.GetConversationById(conversationId);
                var nodes = relatedMessagesTask
                    .Select(x => new MessageNode(this, x) { ShowEndpoints = ShowEndpoints })
                    .ToList();

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
                if (view != null) view.UpdateNode(node);
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
    }
}