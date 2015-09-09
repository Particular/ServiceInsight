namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Drawing;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;

    public class ModelCreator
    {
        readonly List<ReceivedMessage> messages;

        Dictionary<string, HandlerViewModel> handlerRegistar = new Dictionary<string, HandlerViewModel>();

        public ModelCreator(List<ReceivedMessage> messages)
        {
            this.messages = messages;
        }

        public List<EndpointViewModel> GetModel()
        {
            var endpoints = CreateEndpointList();
            PopulateHandlersList(endpoints);
            PopulateHandlerArrows(endpoints);

            return endpoints;
        }

        List<EndpointViewModel> CreateEndpointList()
        {
            var endpoints = new List<OrderData>();

            foreach (var endpointViewModel in messages.Where(m => m.receiving_endpoint != null)
                .Select(m => OrderData.Create(m.message_id, GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null), new EndpointViewModel(m.receiving_endpoint.name, m.receiving_endpoint.host, m.sending_endpoint.Equals(m.receiving_endpoint) ? GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null) : null)))
                .Where(endpointViewModel => !endpoints.Select(t => t.Model).Contains(endpointViewModel.Model)))
            {

                endpoints.Add(endpointViewModel);
            }

            foreach (var endpointViewModel in messages.Where(m => m.sending_endpoint != null)
                .Select(m => OrderData.Create("N/A", null, new EndpointViewModel(m.sending_endpoint.name, m.sending_endpoint.host, GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null))))
                .Where(endpointViewModel => !endpoints.Select(t => t.Model).Contains(endpointViewModel.Model)))
            {
                endpoints.Add(endpointViewModel);
            }

            return Sort(endpoints);
        }

        void PopulateHandlersList(List<EndpointViewModel> endpoints)
        {
            foreach (var message in messages.OrderByDescending(m => GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null)
                .ThenBy(m => m.processed_at))
            {
                if (message.receiving_endpoint == null)
                {
                    continue;
                }

                var endpointViewModel = endpoints.Find(e => IsSameEndpoint(e, message.receiving_endpoint, message.sending_endpoint.Equals(message.receiving_endpoint) ? GetHeaderByKey(message.headers, MessageHeaderKeys.Version, null) : null));
                var handlerViewModel = CreateHandler(message);

                endpointViewModel.Handlers.Add(handlerViewModel);
            }
        }

        void PopulateHandlerArrows(List<EndpointViewModel> endpoints)
        {
            foreach (var message in messages)
            {
                var relatedTo = GetHeaderByKey(message.headers, MessageHeaderKeys.RelatedTo, null);

                if (relatedTo == null)
                {
                    continue;
                }

                HandlerViewModel handler;
                if (handlerRegistar.TryGetValue(relatedTo, out handler))
                {
                    handler.Out.Add(CreateArrow(message));
                }
                else
                {
                    if (message.sending_endpoint == null)
                    {
                        continue;
                    }

                    var endpointViewModel = endpoints.Find(e => IsSameEndpoint(e, message.sending_endpoint, GetHeaderByKey(message.headers, MessageHeaderKeys.Version, null)));

                    if (endpointViewModel.Handlers.Count == 0)
                    {
                        handler = CreateHandler(message);

                        endpointViewModel.Handlers.Add(handler);
                    }
                    else
                    {
                        handler = endpointViewModel.Handlers.Single();
                    }

                    handler.Out.Add(CreateArrow(message));
                }
            }
        }

        HandlerViewModel CreateHandler(ReceivedMessage message)
        {
            var handlerViewModel = new HandlerViewModel
            {
                HandledAt = message.processed_at,
                In = CreateArrow(message),
                Title = message.message_type,
            };

            if (!handlerRegistar.ContainsKey(message.message_id))
            {
                handlerRegistar.Add(message.message_id, handlerViewModel);
            }

            if (message.invoked_sagas != null && message.invoked_sagas.Count > 0)
            {
                handlerViewModel.PartOfSaga = TypeHumanizer.ToName(message.invoked_sagas[0].saga_type);
            }

            if (message.status == MessageStatus.ArchivedFailure || message.status == MessageStatus.Failed || message.status == MessageStatus.RepeatedFailure)
            {
                handlerViewModel.State = HandlerStateType.Fail;
            }
            else
            {
                handlerViewModel.State = HandlerStateType.Success;
            }


            return handlerViewModel;
        }

        List<EndpointViewModel> Sort(IEnumerable<OrderData> endpoints)
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

        static ArrowViewModel CreateArrow(ReceivedMessage message)
        {
            var arrow = new ArrowViewModel
            {
                Title = TypeHumanizer.ToName(message.message_type)
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

        static bool IsSameEndpoint(EndpointViewModel e1, EndpointAddress e2, string version = null)
        {
            if (e1 == null)
            {
                return false;
            }

            if (e2 == null)
            {
                return false;
            }

            return new EndpointViewModel(e2.name, e2.host, version).Equals(e1);
        }

        class OrderData
        {
            public string RelatedTo { get; set; }
            public string MessageId { get; set; }
            public EndpointViewModel Model { get; set; }

            public static OrderData Create(string messageId, string relatedTo, EndpointViewModel model)
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