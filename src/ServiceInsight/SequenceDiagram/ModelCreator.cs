namespace ServiceInsight.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using ServiceInsight.Framework;
    using ServiceInsight.Models;
    using ServiceInsight.SequenceDiagram.Diagram;

    public class ModelCreator
    {
        public const string ConversationStartHandlerName = "First";

        readonly List<StoredMessage> messages;
        readonly IMessageCommandContainer container;

        List<Handler> handlers;
        List<EndpointItem> endpoints = new List<EndpointItem>();
        List<MessageProcessingRoute> processingRoutes = new List<MessageProcessingRoute>();

        public ModelCreator(List<StoredMessage> messages, IMessageCommandContainer container)
        {
            this.messages = messages;
            this.container = container;

            Initialize();
        }

        public ReadOnlyCollection<EndpointItem> Endpoints => endpoints.AsReadOnly();

        public ReadOnlyCollection<Handler> Handlers => handlers.AsReadOnly();

        public ReadOnlyCollection<MessageProcessingRoute> Routes => processingRoutes.AsReadOnly();

        void Initialize()
        {
            var endpointRegistry = new EndpointRegistry();
            var handlerRegistry = new HandlerRegistry();
            var firstOrderHandlers = new List<Handler>();

            var messageTrees = CreateMessageTrees(messages).ToArray();
            var messagesInOrder = messageTrees.SelectMany(x => x.Walk()).ToArray();

            // NOTE: All sending endpoints are created first to ensure version info is retained
            foreach (var message in messagesInOrder)
            {
                endpointRegistry.Register(CreateSendingEndpoint(message));
            }

            foreach (var message in messagesInOrder)
            {
                endpointRegistry.Register(CreateProcessingEndpoint(message));
            }

            foreach (var message in messagesInOrder)
            {
                var sendingEndpoint = endpointRegistry.Get(CreateSendingEndpoint(message));
                if (!endpoints.Contains(sendingEndpoint))
                {
                    endpoints.Add(sendingEndpoint);
                }

                var processingEndpoint = endpointRegistry.Get(CreateProcessingEndpoint(message));
                if (!endpoints.Contains(processingEndpoint))
                {
                    endpoints.Add(processingEndpoint);
                }


                if (handlerRegistry.TryRegisterHandler(CreateSendingHandler(message, sendingEndpoint), out Handler sendingHandler))
                {
                    firstOrderHandlers.Add(sendingHandler);
                    sendingEndpoint.Handlers.Add(sendingHandler);
                }

                sendingHandler.UpdateProcessedAtGuess(message.TimeSent);

                if (handlerRegistry.TryRegisterHandler(CreateProcessingHandler(message, processingEndpoint), out Handler processingHandler))
                {
                    firstOrderHandlers.Add(processingHandler);
                    processingEndpoint.Handlers.Add(processingHandler);
                }
                else
                {
                    UpdateProcessingHandler(processingHandler, message);
                }

                var arrow = CreateArrow(message);
                arrow.ToHandler = processingHandler;
                arrow.FromHandler = sendingHandler;

                var messageProcessingRoute = CreateRoute(arrow, processingHandler);
                arrow.MessageProcessingRoute = messageProcessingRoute;
                processingRoutes.Add(messageProcessingRoute);
                processingHandler.In = arrow;

                sendingHandler.Out = sendingHandler.Out.Concat(new[] { arrow }).OrderBy(a => a).ToList();
            }

            var start = firstOrderHandlers.Where(h => h.ID == ConversationStartHandlerName);
            var orderedByHandledAt = firstOrderHandlers.Where(h => h.ID != ConversationStartHandlerName)
                                                    .OrderBy(h => h.HandledAt)
                                                    .ToList();

            handlers = new List<Handler>(start);
            handlers.AddRange(orderedByHandledAt);
        }

        MessageProcessingRoute CreateRoute(Arrow arrow, Handler processingHandler) => new MessageProcessingRoute(arrow, processingHandler);

        IEnumerable<MessageTreeNode> CreateMessageTrees(IEnumerable<StoredMessage> messages)
        {
            var nodes = messages.Select(x => new MessageTreeNode(x)).ToList();
            var resolved = new HashSet<MessageTreeNode>();
            var index = nodes.ToLookup(x => x.Id);

            foreach (var node in nodes)
            {
                var parent = index[node.Parent].FirstOrDefault();
                if (parent != null)
                {
                    parent.AddChild(node);
                    resolved.Add(node);
                }
            }

            return nodes.Except(resolved);
        }

        static EndpointItem CreateProcessingEndpoint(StoredMessage m) => new EndpointItem(m.ReceivingEndpoint.Name, m.ReceivingEndpoint.Host, m.ReceivingEndpoint.HostId, m.SendingEndpoint.Equals(m.ReceivingEndpoint) ? m.GetHeaderByKey(MessageHeaderKeys.Version, null) : null);

        static EndpointItem CreateSendingEndpoint(StoredMessage m) => new EndpointItem(m.SendingEndpoint.Name, m.SendingEndpoint.Host, m.SendingEndpoint.HostId, m.GetHeaderByKey(MessageHeaderKeys.Version, null));

        Handler CreateSendingHandler(StoredMessage message, EndpointItem sendingEndpoint)
        {
            var handler = new Handler(message.GetHeaderByKey(MessageHeaderKeys.RelatedTo, ConversationStartHandlerName), container)
            {
                State = HandlerState.Success,
                Endpoint = sendingEndpoint
            };

            return handler;
        }

        Handler CreateProcessingHandler(StoredMessage message, EndpointItem processingEndpoint)
        {
            var handler = new Handler(message.MessageId, container)
            {
                Endpoint = processingEndpoint
            };

            UpdateProcessingHandler(handler, message);

            return handler;
        }

        void UpdateProcessingHandler(Handler processingHandler, StoredMessage message)
        {
            processingHandler.ProcessedAt = message.ProcessedAt;
            processingHandler.ProcessingTime = message.ProcessingTime;
            processingHandler.Name = TypeHumanizer.ToName(message.MessageType);

            if (message.InvokedSagas != null && message.InvokedSagas.Count > 0)
            {
                processingHandler.PartOfSaga = string.Join(", ", Array.ConvertAll(message.InvokedSagas.ToArray(), x => TypeHumanizer.ToName(x.SagaType)));
            }

            if (message.Status == MessageStatus.ArchivedFailure || message.Status == MessageStatus.Failed || message.Status == MessageStatus.RepeatedFailure)
            {
                processingHandler.State = HandlerState.Fail;
            }
            else
            {
                processingHandler.State = HandlerState.Success;
            }
        }

        Arrow CreateArrow(StoredMessage message)
        {
            var arrow = new Arrow(message, container)
            {
                Name = TypeHumanizer.ToName(message.MessageType)
            };

            if (message.MessageIntent == MessageIntent.Publish)
            {
                arrow.Type = ArrowType.Event;
            }
            else
            {
                var isTimeoutString = message.GetHeaderByKey(MessageHeaderKeys.IsSagaTimeout);
                var isTimeout = !string.IsNullOrEmpty(isTimeoutString) && bool.Parse(isTimeoutString);
                if (isTimeout)
                {
                    arrow.Type = ArrowType.Timeout;
                }
                else if (Equals(message.ReceivingEndpoint, message.SendingEndpoint))
                {
                    arrow.Type = ArrowType.Local;
                }
                else
                {
                    arrow.Type = ArrowType.Command;
                }
            }

            return arrow;
        }

        class EndpointRegistry
        {
            IDictionary<string, EndpointItem> store = new Dictionary<string, EndpointItem>();

            public void Register(EndpointItem item)
            {
                var key = item.FullName;
                if (!store.TryGetValue(key, out EndpointItem endpoint))
                {
                    store[key] = item;
                    endpoint = item;
                }

                foreach (var host in item.Hosts)
                {
                    endpoint.AddHost(host);
                }
            }

            public EndpointItem Get(EndpointItem item)
            {
                var key = item.FullName;
                var candidate = store[key];

                return candidate;
            }
        }

        class HandlerRegistry
        {
            IDictionary<Tuple<string, EndpointItem>, Handler> handlersLookup = new Dictionary<Tuple<string, EndpointItem>, Handler>();

            public bool TryRegisterHandler(Handler newHandler, out Handler handler)
            {
                var key = Tuple.Create(newHandler.ID, newHandler.Endpoint);
                if (handlersLookup.TryGetValue(key, out Handler existingHandler))
                {
                    handler = existingHandler;
                    return false;
                }

                handlersLookup.Add(key, newHandler);

                handler = newHandler;
                return true;
            }
        }

        class MessageTreeNode
        {
            List<MessageTreeNode> children = new List<MessageTreeNode>();

            public MessageTreeNode(StoredMessage msg)
            {
                Message = msg;
                Parent = msg.GetHeaderByKey(MessageHeaderKeys.RelatedTo, null);
            }

            public string Id => Message.MessageId;

            public string Parent { get; set; }

            public StoredMessage Message { get; set; }

            public IEnumerable<MessageTreeNode> Children => children;

            public void AddChild(MessageTreeNode childNode)
            {
                children.Add(childNode);
            }

            public IEnumerable<StoredMessage> Walk()
            {
                yield return Message;
                foreach (var child in Children.OrderBy(x => x.Message.ProcessedAt))
                {
                    foreach (var walked in child.Walk())
                    {
                        yield return walked;
                    }
                }
            }
        }
    }
}