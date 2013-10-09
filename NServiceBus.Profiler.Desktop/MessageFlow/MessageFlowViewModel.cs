using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Mindscape.WpfDiagramming;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Management;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    public interface IMessageFlowViewModel : IScreen, 
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        MessageFlowDiagram Diagram { get; }
    }

    public class MessageFlowViewModel : Screen, IMessageFlowViewModel
    {
        private readonly IManagementService _managementService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ConcurrentDictionary<string, MessageNode> _nodeMap;
        private IMessageFlowView _view;

        public MessageFlowViewModel(
            IManagementService managementService,
            IEventAggregator eventAggregator)
        {
            _managementService = managementService;
            _eventAggregator = eventAggregator;
            Diagram = new MessageFlowDiagram();
            _nodeMap = new ConcurrentDictionary<string, MessageNode>();
        }

        public MessageFlowDiagram Diagram
        {
            get; set;
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IMessageFlowView)view;
        }

        public void ToggleEndpointData()
        {
            ShowEndpoints = !ShowEndpoints;
        }

        public async void Handle(MessageBodyLoaded @event)
        {
            var storedMessage = @event.Message as StoredMessage;
            if (storedMessage != null)
            {
                var conversationId = storedMessage.ConversationId;
                if (conversationId == null) return;

                _eventAggregator.Publish(new WorkStarted("Loading conversation data..."));

                var relatedMessagesTask = await _managementService.GetConversationById(conversationId);
                var nodes = relatedMessagesTask.ConvertAll(CreateMessageNode);

                CreateConversationNodes(storedMessage.Id, nodes);
                LinkConversationNodes(nodes);
                UpdateLayout();

                _eventAggregator.Publish(new WorkFinished());
            }
        }

        private MessageNode CreateMessageNode(StoredMessage x)
        {
            return new MessageNode(x) { DisplayEndpointInformation = ShowEndpoints };
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
                    m.Message.MessageId == msg.Message.RelatedToMessageId && 
                    m.Message.ReceivingEndpoint.Name == msg.Message.OriginatingEndpoint.Name);

                if (parentMessage == null)
                    continue;

                AddConnection(parentMessage, msg);
            }
        }

        private void AddConnection(MessageNode from, MessageNode to)
        {
            var fromPoint = new DiagramConnectionPoint(from, Edge.Bottom);
            var toPoint = new DiagramConnectionPoint(to, Edge.Top);
            
            from.ConnectionPoints.Add(fromPoint);
            to.ConnectionPoints.Add(toPoint);

            Diagram.Connections.Add(new FlowDiagramConnection(fromPoint, toPoint));
        }

        private void CreateConversationNodes(string selectedId, IEnumerable<MessageNode> relatedNodes)
        {
            foreach (var node in relatedNodes)
            {
                if (string.Equals(node.Message.Id, selectedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    SelectedMessage = node;
                }

                _nodeMap.TryAdd(node.Message.Id, node);
                Diagram.Nodes.Add(node);
            }
        }

        public bool ShowEndpoints
        {
            get; set;
        }

        public void OnShowEndpointsChanged()
        {
            foreach (var node in Diagram.Nodes.OfType<MessageNode>())
            {
                node.DisplayEndpointInformation = ShowEndpoints;
            }
        }

        private void UpdateLayout()
        {
            if (_view != null)
            {
                _view.ApplyLayout();
            }
        }

        public MessageNode SelectedMessage
        {
            get; set;
        }

        public void Handle(SelectedMessageChanged message)
        {
            SelectedMessage = null;
            _nodeMap.Clear();
            Diagram = new MessageFlowDiagram();
        }
    }
}