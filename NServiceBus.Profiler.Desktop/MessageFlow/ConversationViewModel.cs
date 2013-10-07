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
    public interface IConversationViewModel : IScreen, 
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        MessageFlowDiagram Diagram { get; }
    }

    public class ConversationViewModel : Screen, IConversationViewModel
    {
        private readonly IManagementService _managementService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ConcurrentDictionary<string, MessageNode> _nodeMap;
        private IConversationView _view;

        public ConversationViewModel(
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
            _view = (IConversationView)view;
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
                var nodes = relatedMessagesTask.ConvertAll(x => new MessageNode(x));

                CreateConversationNodes(storedMessage.Id, nodes);
                LinkConversationNodes(nodes);
                UpdateLayout();

                _eventAggregator.Publish(new WorkFinished());
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
                    m.Message.MessageId == msg.Message.RelatedToMessageId && 
                    m.Message.ReceivingEndpoint.Name == msg.Message.OriginatingEndpoint.Name);

                if (parentMessage == null)
                    continue;

                AddConnection(parentMessage, msg);
            }
        }

        private void AddConnection(MessageNode from, MessageNode to)
        {
            var fromPoint = new DiagramConnectionPoint(from, Edge.Right);
            var toPoint = new DiagramConnectionPoint(to, Edge.Left);
            
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