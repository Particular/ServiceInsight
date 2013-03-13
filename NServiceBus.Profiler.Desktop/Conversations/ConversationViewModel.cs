using System.Collections.Concurrent;
using System.Collections.Generic;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationViewModel : Screen, IConversationViewModel
    {
        private readonly IManagementService _managementService;
        private readonly IEndpointConnectionProvider _connection;
        private readonly ConcurrentDictionary<string, StoredMessage> _nodeMap;
        private IConversationView _view;
        private object Locker = new object();

        public ConversationViewModel(IManagementService managementService, IEndpointConnectionProvider connection)
        {
            _managementService = managementService;
            _connection = connection;
            _nodeMap = new ConcurrentDictionary<string, StoredMessage>();

            Graph = new ConversationGraph();
        }

        private void NewGraphEdge(StoredMessage from, StoredMessage to)
        {
            var edge = new MessageEdge(from, to);
            Graph.AddEdge(edge);
        }

        public ConversationGraph Graph { get; set; }

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
                var relatedMessagesTask = await _managementService.GetConversationById(_connection.ServiceUrl, conversationId);

                CreateConversationNodes(relatedMessagesTask);
                LinkConversationNodes(relatedMessagesTask);
            }
        }

        private void LinkConversationNodes(IEnumerable<StoredMessage> relatedMessagesTask)
        {
            foreach (var msg in relatedMessagesTask)
            {
                if (msg.RelatedToMessageId != null && 
                    _nodeMap.ContainsKey(msg.Id) &&
                    _nodeMap.ContainsKey(msg.RelatedToMessageId))
                {
                    var source = _nodeMap[msg.Id];
                    var target = _nodeMap[msg.RelatedToMessageId];

                    if (source != null && target != null)
                    {
                        NewGraphEdge(target, source);
                    }
                }
            }
        }

        private void CreateConversationNodes(IEnumerable<StoredMessage> relatedMessages)
        {
            foreach (var msg in relatedMessages)
            {
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