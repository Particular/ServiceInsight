using System.Collections.Generic;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Desktop.Explorer;
using System.Linq;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public class ConversationViewModel : Screen, IConversationViewModel
    {
        private readonly IManagementService _managementService;
        private readonly IEndpointConnectionProvider _connection;
        private readonly Dictionary<string, ConversationNode> _nodeMap;
        private IConversationView _view;

        public ConversationViewModel(IManagementService managementService, IEndpointConnectionProvider connection)
        {
            _managementService = managementService;
            _connection = connection;
            _nodeMap = new Dictionary<string, ConversationNode>();

            Graph = new ConversationGraph();
        }

        private ConversationNode NewVertex(StoredMessage msg)
        {
            var vertex = new ConversationNode
            {
                OriginatingEndpoint = msg.OriginatingEndpoint.Name,
                ReceivingEndpoint = msg.ReceivingEndpoint.Name,
                Title = GetTypeFriendlyName(msg.MessageType),
                MessageId = msg.Id,
                IsErrorMessage = msg.FailureDetails != null,
            };

            Graph.AddVertex(vertex);

            return vertex;
        }

        private string GetTypeFriendlyName(string messageType)
        {
            if (string.IsNullOrEmpty(messageType))
                return string.Empty;

            var clazz = messageType.Split(',').First();
            var objectName = clazz.Split('.').Last();

            return objectName;
        }

        private void NewGraphEdge(ConversationNode from, ConversationNode to)
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

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ZoomToDefault();
        }

        public void ZoomToDefault()
        {
            if (_view != null)
            {
                _view.ZoomToDefault();
            }
        }

        public void Redraw()
        {
            _view.Redraw();
        }

        public async void Handle(MessageBodyLoadedEvent @event)
        {
            var storedMessage = @event.Message as StoredMessage;
            if (storedMessage != null)
            {
                var conversationId = storedMessage.ConversationId;
                var relatedMessagesTask = await _managementService.GetConversationById(_connection.ConnectedToUrl, conversationId);

                CreateConversationNodes(relatedMessagesTask);
                LinkConversationNodes(relatedMessagesTask);
            }

            ZoomToDefault();
            Redraw();
        }

        private void LinkConversationNodes(IEnumerable<StoredMessage> relatedMessagesTask)
        {
            foreach (var msg in relatedMessagesTask)
            {
                if (msg.RelatedToMessageId != null)
                {
                    var source = _nodeMap[msg.Id];
                    var target = _nodeMap[msg.RelatedToMessageId];

                    if (source != null && target != null)
                    {
                        NewGraphEdge(source, target);
                    }
                }
            }
        }

        private void CreateConversationNodes(IEnumerable<StoredMessage> relatedMessages)
        {
            foreach (var msg in relatedMessages)
            {
                var vertex = NewVertex(msg);
                _nodeMap.Add(msg.Id, vertex);
            }
        }

        public void Handle(SelectedMessageChangedEvent message)
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