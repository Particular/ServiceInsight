namespace ServiceInsight.MessageFlow
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Autofac;
    using Caliburn.Micro;
    using Mindscape.WpfDiagramming;
    using Mindscape.WpfDiagramming.FlowDiagrams;
    using Models;
    using ServiceControl;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.MessageList;
    using Settings;

    public class MessageFlowViewModel : Screen,
        IHandleWithTask<SelectedMessageChanged>
    {
        Func<ExceptionDetailViewModel> exceptionDetail;
        IServiceControl serviceControl;
        IEventAggregator eventAggregator;
        IWindowManagerEx windowManager;
        ISettingsProvider settingsProvider;
        MessageSelectionContext selection;
        ConcurrentDictionary<string, MessageNode> nodeMap;
        MessageFlowView view;
        string loadedConversationId;
        private IWorkNotifier workNotifier;

        public MessageFlowViewModel(
            IServiceControl serviceControl,
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            IContainer container,
            Func<ExceptionDetailViewModel> exceptionDetail,
            ISettingsProvider settingsProvider,
            MessageSelectionContext selectionContext,
            IWorkNotifier workNotifier)
        {
            this.workNotifier = workNotifier;
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            this.windowManager = windowManager;
            this.settingsProvider = settingsProvider;
            selection = selectionContext;
            this.exceptionDetail = exceptionDetail;

            CopyConversationIDCommand = container.Resolve<CopyConversationIDCommand>();
            CopyMessageURICommand = container.Resolve<CopyMessageURICommand>();
            RetryMessageCommand = container.Resolve<RetryMessageCommand>();
            SearchByMessageIDCommand = container.Resolve<SearchByMessageIDCommand>();

            Diagram = new FlowDiagramModel();
            nodeMap = new ConcurrentDictionary<string, MessageNode>();
        }

        public FlowDiagramModel Diagram
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
            this.view = (MessageFlowView)view;
            this.view.ShowMessage += OnShowMessage;
        }

        void OnShowMessage(object sender, SearchMessageEventArgs e)
        {
            if (e.MessageNode == null)
            {
                return;
            }

            var message = e.MessageNode.Message;

            eventAggregator.PublishOnUIThread(new RequestSelectingEndpoint(message.ReceivingEndpoint));
            selection.SelectedMessage = message;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var settings = settingsProvider.GetSettings<ProfilerSettings>();

            ShowEndpoints = settings.ShowEndpoints;
        }

        public void ShowMessageBody()
        {
            eventAggregator.PublishOnUIThread(SwitchToMessageBody.Instance);
        }

        public void ShowSagaWindow()
        {
            if (SelectedMessage != null)
            {
                selection.SelectedMessage = SelectedMessage.Message;
            }
            eventAggregator.PublishOnUIThread(SwitchToSagaWindow.Instance);
        }

        public void ShowException(ExceptionDetails exception)
        {
            var model = exceptionDetail();
            model.Exception = exception;
            windowManager.ShowDialog(model);
        }

        public void ToggleEndpointData()
        {
            ShowEndpoints = !ShowEndpoints;
        }

        public ICommand CopyConversationIDCommand { get; }

        public ICommand CopyMessageURICommand { get; }

        public ICommand SearchByMessageIDCommand { get; }

        public ICommand RetryMessageCommand { get; }

        public async Task Handle(SelectedMessageChanged @event)
        {
            var storedMessage = selection.SelectedMessage;
            if (storedMessage == null)
            {
                ClearState();
                return;
            }

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
            {
                ClearState();
                return;
            }

            if (loadedConversationId == conversationId)
            {
                RefreshSelection(storedMessage.Id);
                return;
            }

            ClearState();

            loadedConversationId = conversationId;

            using (workNotifier.NotifyOfWork("Loading flow..."))
            {
                var relatedMessagesTask = await serviceControl.GetConversationById(conversationId);
                var nodes = relatedMessagesTask
                    .Select(x => new MessageNode(this, x)
                    {
                        ShowEndpoints = ShowEndpoints,
                        IsFocused = x.Id == storedMessage.Id
                    })
                    .ToList();

                CreateConversationNodes(storedMessage.Id, nodes);
                LinkConversationNodes(nodes);
                UpdateLayout();
            }
        }

        void RefreshSelection(string selectedId)
        {
            foreach (var node in Diagram.Nodes.OfType<MessageNode>())
            {
                if (string.Equals(node.Message.Id, selectedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    node.IsFocused = true;
                    SelectedMessage = node;
                    continue;
                }

                node.IsFocused = false;
            }
        }

        void ClearState()
        {
            nodeMap.Clear();

            loadedConversationId = null;
            SelectedMessage = null;
            Diagram = new FlowDiagramModel();
        }

        public void ZoomIn()
        {
            view.Surface.Zoom += 0.1;
        }

        public void ZoomOut()
        {
            view.Surface.Zoom -= 0.1;
        }

        public void OnShowEndpointsChanged()
        {
            foreach (var node in Diagram.Nodes.OfType<MessageNode>())
            {
                node.ShowEndpoints = ShowEndpoints;
            }

            UpdateSetting();
            UpdateLayout();
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

                // [CM] I don't know how it's happening, but a user reported an
                // error where multiple results were returned from this query.
                var parentMessages = nodeMap.Values.Where(m =>
                    m.Message != null && m.Message.ReceivingEndpoint != null && m.Message.SendingEndpoint != null &&
                    m.Message.MessageId == msg.Message.RelatedToMessageId &&
                    m.Message.ReceivingEndpoint.Name == msg.Message.SendingEndpoint.Name);

                foreach (var parentMessage in parentMessages)
                {
                    AddConnection(parentMessage, msg);
                }
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
            else if (childNode.IsTimeout)
            {
                connection = new TimeoutConnection(fromPoint, toPoint);
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
            }
        }
    }
}