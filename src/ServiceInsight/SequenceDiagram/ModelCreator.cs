namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Drawing;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;

    class ModelCreator
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
            PopulateHandlerArrows();

            return endpoints;
        }

        List<EndpointViewModel> CreateEndpointList()
        {
            var endpoints = new List<EndpointViewModel>();

            foreach (var endpointViewModel in messages.Where(m => m.sending_endpoint != null)
                .OrderBy(m => GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null)
                .ThenBy(m => m.processed_at)
                .Select(m => new EndpointViewModel(m.sending_endpoint.name, m.sending_endpoint.host, GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null ? DateTime.MinValue : m.processed_at, GetHeaderByKey(m.headers, MessageHeaderKeys.Version, null)))
                .Where(endpointViewModel => !endpoints.Contains(endpointViewModel)))
            {
                endpoints.Add(endpointViewModel);
            }

            foreach (var endpointViewModel in messages.Where(m => m.receiving_endpoint != null)
                .OrderBy(m => GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null)
                .ThenBy(m => m.processed_at)
                .Select(m => new EndpointViewModel(m.receiving_endpoint.name, m.receiving_endpoint.host, GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null ? DateTime.MinValue : m.processed_at))
                .Where(endpointViewModel => !endpoints.Contains(endpointViewModel)))
            {
                endpoints.Add(endpointViewModel);
            }

            return endpoints.OrderBy(vm => vm.Order).ToList();
        }

        void PopulateHandlersList(List<EndpointViewModel> endpoints)
        {
            foreach (var message in messages.OrderBy(m => GetHeaderByKey(m.headers, MessageHeaderKeys.RelatedTo, null) == null)
                .ThenBy(m => m.processed_at))
            {
                if (message.receiving_endpoint == null)
                {
                    continue;
                }

                var endpointViewModel = endpoints.Find(e => IsSameEndpoint(e, message.receiving_endpoint, GetHeaderByKey(message.headers, MessageHeaderKeys.Version, null)));
                var handlerViewModel = CreateHandler(message);

                endpointViewModel.Handlers.Add(handlerViewModel);
            }
        }

        void PopulateHandlerArrows()
        {
            foreach (var message in messages)
            {
                var relatedTo = GetHeaderByKey(message.headers, MessageHeaderKeys.RelatedTo, null);

                HandlerViewModel handler;
                if (handlerRegistar.TryGetValue(relatedTo, out handler))
                {
                    handler.Out.Add(CreateArrow(message));
                }
            }
        }

        HandlerViewModel CreateHandler(ReceivedMessage message)
        {
            var handlerViewModel = new HandlerViewModel
            {
                HandledAt = message.processed_at,
                In = CreateArrow(message)
            };

            handlerRegistar.Add(message.message_id, handlerViewModel);

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

            return new EndpointViewModel(e2.name, e2.host, DateTime.MinValue, version).Equals(e1);
        }
    }
}