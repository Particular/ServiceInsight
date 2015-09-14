namespace ServiceInsight.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Diagram;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;

    public class ModelCreator
    {
        readonly List<ReceivedMessage> messages;

        Dictionary<EndpointItem, OrderData> endpointsLookup = new Dictionary<EndpointItem, OrderData>();
        Dictionary<Tuple<string, EndpointItem>, Handler> handlersLookup = new Dictionary<Tuple<string, EndpointItem>, Handler>();

        public ModelCreator(List<ReceivedMessage> messages)
        {
            this.messages = messages;
        }

        public List<EndpointItem> GetModel()
        {
            var endpoints = CreateEndpointList();
            PopulateHandlers();

            foreach (var endpoint in endpoints)
            {
                endpoint.Handlers.Sort(Comparer<Handler>.Default);
            }

            return endpoints;
        }

        void PopulateHandlers()
        {
            foreach (var message in messages.OrderByDescending(m => GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null)
                .ThenBy(m => m.processed_at))
            {
                var processingEndpoint = LookupEndpoint(CreateProcessingEndpoint(message));
                var sendingEndpoint = LookupEndpoint(CreateSendingEndpoint(message));

                Handler processingHandler;
                Handler sendingHandler;

                if (TryRegisterHandler(CreateProcessingHandler(message, processingEndpoint), out processingHandler))
                {
                    processingEndpoint.Handlers.Add(processingHandler);
                }
                if (TryRegisterHandler(CreateSendingHandler(message, sendingEndpoint), out sendingHandler))
                {
                    sendingEndpoint.Handlers.Add(sendingHandler);
                }

                var arrow = CreateArrow(message);

                arrow.ToHandler = processingHandler;
                arrow.FromHandler = sendingHandler;

                processingHandler.In = arrow;

                sendingHandler.Out.Add(arrow);
            }
        }

        List<EndpointItem> CreateEndpointList()
        {
            foreach (var endpointViewModel in messages.Where(m => m.sending_endpoint != null)
                .Select(m => OrderData.Create(m.message_id, GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null), CreateSendingEndpoint(m)))
                .Where(endpointViewModel => !endpointsLookup.Any(_=> _.Key.Equals(endpointViewModel.Model))))
            {
                endpointsLookup.Add(endpointViewModel.Model, endpointViewModel);
            }

            foreach (var endpointViewModel in messages.Where(m => m.receiving_endpoint != null)
                .Select(m => OrderData.Create("NotKnown", m.message_id, CreateProcessingEndpoint(m)))
                .Where(endpointViewModel => !endpointsLookup.Any(_ => _.Key.Equals(endpointViewModel.Model))))
            {
                endpointsLookup.Add(endpointViewModel.Model, endpointViewModel);
            }

            return Sort(endpointsLookup.Values);
        }

        bool TryRegisterHandler(Handler newHandler, out Handler handler)
        {
            Handler existingHandler;
            var key = Tuple.Create(newHandler.ID, newHandler.Endpoint);
            if (handlersLookup.TryGetValue(key, out existingHandler))
            {
                handler = existingHandler;
                return false;
            }

            handlersLookup.Add(key, newHandler);

            handler = newHandler;
            return true;
        }

        EndpointItem LookupEndpoint(EndpointItem newEndpoint)
        {
            return endpointsLookup.Keys.Single(_=>_.Equals(newEndpoint));
        }

        EndpointItem CreateProcessingEndpoint(ReceivedMessage m)
        {
            return new EndpointItem(m.receiving_endpoint.name, m.receiving_endpoint.host, m.receiving_endpoint.host_id, m.sending_endpoint.Equals(m.receiving_endpoint) ? GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null) : null);
        }

        EndpointItem CreateSendingEndpoint(ReceivedMessage m)
        {
            return new EndpointItem(m.sending_endpoint.name, m.sending_endpoint.host, m.sending_endpoint.host_id, GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null));
        }

        Handler CreateSendingHandler(ReceivedMessage message, EndpointItem sendingEndpoint)
        {
            return new Handler(GetHeaderByKey(message.headers, MessageHeaderKeys.RelatedTo, "First"))
            {
                HandledAt = message.time_sent,
                State = HandlerState.Success,
                Endpoint = sendingEndpoint
            };
        }

        Handler CreateProcessingHandler(ReceivedMessage message, EndpointItem processingEndpoint)
        {
            var handler = new Handler(message.message_id)
            {
                HandledAt = message.processed_at,
                Name = message.message_type,
                Endpoint = processingEndpoint
            };

            if (message.invoked_sagas != null && message.invoked_sagas.Count > 0)
            {
                //TODO: Support multiple sagas!
                handler.PartOfSaga = TypeHumanizer.ToName(message.invoked_sagas[0].saga_type);
            }

            if (message.status == MessageStatus.ArchivedFailure || message.status == MessageStatus.Failed || message.status == MessageStatus.RepeatedFailure)
            {
                handler.State = HandlerState.Fail;
            }
            else
            {
                handler.State = HandlerState.Success;
            }

            return handler;
        }

        List<EndpointItem> Sort(IEnumerable<OrderData> endpoints)
        {
            var orderedList = new LinkedList<OrderData>();
            foreach (var endpoint in endpoints)
            {
                var relatedTo = endpoint.RelatedTo;

                if (relatedTo == null)
                {
                    orderedList.AddFirst(endpoint);
                    continue;
                }

                var current = orderedList.First;
                var inserted = false;

                while (current != null)
                {
                    if (relatedTo != current.Value.MessageId)
                    {
                        current = current.Next;
                        continue;
                    }

                    orderedList.AddAfter(current, endpoint);
                    inserted = true;
                    break;
                }

                if (!inserted)
                {
                    orderedList.AddLast(endpoint);
                }
            }

            return orderedList.Select(t => t.Model).ToList();
        }

        static Arrow CreateArrow(ReceivedMessage message)
        {
            var arrow = new Arrow(message.message_id)
            {
                Name = TypeHumanizer.ToName(message.message_type)
            };

            if (message.message_intent == MessageIntent.Publish)
            {
                arrow.Type = ArrowType.Event;
            }
            else
            {
                var isTimeoutString = GetHeaderByKey(message.headers, MessageHeaderKeys.IsSagaTimeout);
                var isTimeout = !string.IsNullOrEmpty(isTimeoutString) && bool.Parse(isTimeoutString);
                if (isTimeout)
                {
                    arrow.Type = ArrowType.Timeout;
                }
                else if (Equals(message.receiving_endpoint, message.sending_endpoint))
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

        static string GetHeaderByKey(IEnumerable<Header> headers, string key, string defaultValue = "")
        {
            //NOTE: Some keys start with NServiceBus, some don't
            var keyWithPrefix = "NServiceBus." + key;
            var pair = headers.FirstOrDefault(x => x.key.Equals(key, StringComparison.InvariantCultureIgnoreCase) ||
                                                   x.key.Equals(keyWithPrefix, StringComparison.InvariantCultureIgnoreCase));
            return pair == null ? defaultValue : pair.value;
        }

        class OrderData
        {
            public string RelatedTo { get; set; }
            public string MessageId { get; set; }
            public EndpointItem Model { get; set; }

            public static OrderData Create(string messageId, string relatedTo, EndpointItem model)
            {
                return new OrderData
                {
                    MessageId = messageId,
                    RelatedTo = relatedTo,
                    Model = model
                };
            }
        }
    }
}