namespace ServiceInsight.MessageFlow
{
    using ServiceInsight.Explorer;
    using ServiceInsight.ExtensionMethods;
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
    using Anotar.Serilog;
    using ServiceInsight.Models;
    using ServiceInsight.ServiceControl;
    using ServiceInsight.Framework;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.MessageList;
    using ServiceInsight.Settings;

    public class MessageFlowViewModel : Screen,
        IHandleWithTask<SelectedMessageChanged>,
        IHandle<SelectedExplorerItemChanged>
    {
        readonly Func<ExceptionDetailViewModel> exceptionDetail;
        readonly IEventAggregator eventAggregator;
        readonly IServiceInsightWindowManager windowManager;
        readonly ISettingsProvider settingsProvider;
        readonly MessageSelectionContext selection;
        readonly ConcurrentDictionary<string, MessageNode> nodeMap;
        readonly IWorkNotifier workNotifier;
        readonly ServiceControlClientRegistry clientRegistry;

        MessageFlowView view;
        string loadedConversationId;
        ExplorerItem selectedExplorerItem;

        public MessageFlowViewModel(
            IEventAggregator eventAggregator,
            IServiceInsightWindowManager windowManager,
            ILifetimeScope container,
            Func<ExceptionDetailViewModel> exceptionDetail,
            ISettingsProvider settingsProvider,
            MessageSelectionContext selectionContext,
            IWorkNotifier workNotifier,
            ServiceControlClientRegistry clientRegistry)
        {
            this.workNotifier = workNotifier;
            this.clientRegistry = clientRegistry;
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

        IServiceControl ServiceControl => selectedExplorerItem.GetServiceControlClient(clientRegistry);

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
                if (ServiceControl != null)
                {
                    var relatedMessagesTask = await ServiceControl.GetConversationById(conversationId, ApplicationConfiguration.ConversationPageSize);
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
        }

        public void Handle(SelectedExplorerItemChanged @event)
        {
            selectedExplorerItem = @event.SelectedExplorerItem;
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
                if (string.IsNullOrEmpty(msg.Message.RelatedToMessageId) && msg.Message.RelatedToMessageId != msg.Message.MessageId)
                {
                    continue;
                }

                var parentMessages = nodeMap.Values.Where(m =>
                    m.Message != null && m.Message.ReceivingEndpoint != null && m.Message.SendingEndpoint != null
                    && m.Message.MessageId == msg.Message.RelatedToMessageId
                    && m.Message.ReceivingEndpoint.Name == msg.Message.SendingEndpoint.Name // Needed with publishes as identical events can be consumed by multiple endpoints
                    );

                // Fallback, get "parent" when originating message is not an event (publish)
                if (!parentMessages.Any())
                {
                    parentMessages = nodeMap.Values.Where(m =>
                        m.Message != null && m.Message.ReceivingEndpoint != null && m.Message.SendingEndpoint != null &&
                        m.Message.MessageId == msg.Message.RelatedToMessageId && m.Message.MessageIntent != MessageIntent.Publish
                        );

                    if (parentMessages.Any())
                    {
                        LogTo.Warning("Fall back to match only on RelatedToMessageId for message {0} matched but link could be invalid.", msg.Message.MessageId);
                    }
                }

                if (!parentMessages.Any())
                {
                    LogTo.Error("No parent could be resolved for message {0} which has RelatedToMessageId set. This can happen if the parent has been purged due to retention expiration, an ServiceControl node to be unavailable, or because the parent message not been stored (yet).", msg.Message.MessageId);
                }
                else if (parentMessages.Count() > 1)
                {
                    LogTo.Error("Multiple parents matched for {0} possibly due to more-than-once processing, linking to all as unknown which processing attemps generated the message.", msg.Message.MessageId);
                }

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
            view?.ApplyLayout();
        }
    }
}
