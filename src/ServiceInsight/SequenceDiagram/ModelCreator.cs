namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Diagram;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;

    public class ModelCreator
    {
        readonly List<ReceivedMessage> messages;

        Dictionary<string, Handler> handlerRegistar = new Dictionary<string, Handler>();

        public ModelCreator(List<ReceivedMessage> messages)
        {
            this.messages = messages;
        }

        public List<EndpointItem> GetModel()
        {
            var endpoints = CreateEndpointList();
            PopulateHandlersList(endpoints);
            PopulateHandlerArrows(endpoints);

            return endpoints;
        }

        List<EndpointItem> CreateEndpointList()
        {
            var endpoints = new List<OrderData>();

            foreach (var endpointViewModel in messages.Where(m => m.sending_endpoint != null)
                .Select(m => OrderData.Create(m.message_id, GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null), new EndpointItem(m.sending_endpoint.name, m.sending_endpoint.host, GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null))))
                .Where(endpointViewModel => !endpoints.Select(t => t.Model).Contains(endpointViewModel.Model)))
            {
                endpoints.Add(endpointViewModel);
            }

            foreach (var endpointViewModel in messages.Where(m => m.receiving_endpoint != null)
                .Select(m => OrderData.Create("NotKnown", m.message_id, new EndpointItem(m.receiving_endpoint.name, m.receiving_endpoint.host, m.sending_endpoint.Equals(m.receiving_endpoint) ? GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null) : null)))
                .Where(endpointViewModel => !endpoints.Select(t => t.Model).Contains(endpointViewModel.Model)))
            {

                endpoints.Add(endpointViewModel);
            }

            return Sort(endpoints);
        }

        void PopulateHandlersList(List<EndpointItem> endpoints)
        {
            foreach (var message in messages.OrderByDescending(m => GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null)
                .ThenBy(m => m.processed_at))
            {
                if (message.receiving_endpoint == null)
                {
                    continue;
                }

                var endpointItem = endpoints.Find(e => IsSameEndpoint(e, message.receiving_endpoint, message.sending_endpoint.Equals(message.receiving_endpoint) ? GetHeaderByKey(message.headers, MessageHeaderKeys.Version, null) : null));
                var handler = CreateHandler(message);

                endpointItem.Handlers.Add(handler);
            }
        }

        void PopulateHandlerArrows(List<EndpointItem> endpoints)
        {
            foreach (var message in messages)
            {
                var relatedTo = GetHeaderByKey(message.headers, MessageHeaderKeys.RelatedTo, null);

                if (relatedTo == null)
                {
                    if (message.sending_endpoint != null)
                    {
                        CreateHandlerFromSendingEndpoint(endpoints, message, m => new Handler
                        {
                            State = HandlerState.Success,
                            HandledAt = m.time_sent
                        });
                    }
                    continue;
                }

                Handler handler;
                if (handlerRegistar.TryGetValue(relatedTo, out handler))
                {
                    handler.Out.Add(CreateArrow(message));
                }
                else
                {
                    if (message.sending_endpoint != null)
                    {
                        CreateHandlerFromSendingEndpoint(endpoints, message, CreateHandler);
                    }
                }
            }
        }

        void CreateHandlerFromSendingEndpoint(List<EndpointItem> endpoints, ReceivedMessage message, Func<ReceivedMessage, Handler> handlerFactory)
        {
            Handler handler;
            var endpointItems = endpoints.Find(e => IsSameEndpoint(e, message.sending_endpoint, GetHeaderByKey(message.headers, MessageHeaderKeys.Version, null)));

            var relatedTo = GetHeaderByKey(message.headers, MessageHeaderKeys.RelatedTo, null);
            if (relatedTo == null || endpointItems.Handlers.Count == 0)
            {
                handler = handlerFactory(message);

                endpointItems.Handlers.Add(handler);
            }
            else
            {
                if (handlerRegistar.ContainsKey(relatedTo))
                {
                    handler = handlerRegistar[relatedTo];
                }
                else
                {
                    handler = endpointItems.Handlers.Single();
                }
            }

            handler.Out.Add(CreateArrow(message));
        }

        Handler CreateHandler(ReceivedMessage message)
        {
            var handler = new Handler
            {
                HandledAt = message.processed_at,
                In = CreateArrow(message),
                Name = message.message_type,
            };

            if (!handlerRegistar.ContainsKey(message.message_id))
            {
                handlerRegistar.Add(message.message_id, handler);
            }

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
            var arrow = new Arrow
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
                else if (message.receiving_endpoint == message.sending_endpoint)
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

        static bool IsSameEndpoint(EndpointItem e1, EndpointAddress e2, string version = null)
        {
            if (e1 == null)
            {
                return false;
            }

            if (e2 == null)
            {
                return false;
            }

            return new EndpointItem(e2.name, e2.host, version).Equals(e1);
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