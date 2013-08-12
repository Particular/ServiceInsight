﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using System.Diagnostics;
using System.Linq;
using Particular.ServiceInsight.Desktop.Conversations;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Management;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Conversations
{

    public class ConversationViewModel : Screen, IConversationViewModel
    {
        private readonly IManagementService _managementService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ConcurrentDictionary<string, DiagramNode> _nodeMap;
        private IConversationView _view;

        public ConversationViewModel(
            IManagementService managementService,
            IEventAggregator eventAggregator)
        {
            _managementService = managementService;
            _eventAggregator = eventAggregator;
            _nodeMap = new ConcurrentDictionary<string, DiagramNode>();

            Graph = new ConversationGraph();
        }

        private void NewGraphEdge(DiagramNode from, DiagramNode to)
        {
            var edge = new MessageEdge(from, to);
            Graph.AddEdge(edge);
        }

        public ConversationGraph Graph { get; set; }

        public void GraphLayoutUpdated()
        {
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
                var nodes = relatedMessagesTask.ConvertAll(x => new DiagramNode(x));

                CreateConversationNodes(storedMessage.Id, nodes);
                LinkConversationNodes(nodes);

                _eventAggregator.Publish(new WorkFinished());
            }
        }

        private void LinkConversationNodes(IEnumerable<DiagramNode> relatedMessagesTask)
        {
            foreach (var msg in relatedMessagesTask)
            {
                Debug.WriteLine("Processing message type: " + msg.MessageType);

                if (msg.RelatedToMessageId == null && msg.RelatedToMessageId != msg.MessageId)
                    continue;

                var parentMessage = _nodeMap.Values.SingleOrDefault(m => 
                    m.MessageId == msg.RelatedToMessageId && 
                    m.ReceivingEndpoint.Name == msg.OriginatingEndpoint.Name);

                if (parentMessage == null)
                    continue;

                NewGraphEdge(parentMessage, msg);
            }
        }

        private void CreateConversationNodes(string selectedId, IEnumerable<DiagramNode> relatedMessages)
        {
            foreach (var msg in relatedMessages)
            {
                msg.IsCurrentMessage = String.Equals(msg.Id, selectedId, StringComparison.InvariantCultureIgnoreCase);

                Graph.AddVertex(msg);
                _nodeMap.TryAdd(msg.Id, msg);
            }
        }

        public void Handle(SelectedMessageChanged message)
        {
            _nodeMap.Clear();
            Graph.Clear();

            if (_view != null)
            {
                _view.Clear();
            }
        }
    }
}