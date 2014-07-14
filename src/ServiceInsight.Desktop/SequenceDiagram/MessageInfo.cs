namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using ReactiveUI;

    public class MessageInfo : ReactiveObject
    {
        public MessageInfo(StoredMessage message, IEnumerable<EndpointInfo> endpoints)
        {
            Endpoints = endpoints;

            Name = message.FriendlyMessageType;
            if (message.Sagas != null && message.Sagas.Any())
                SagaName = message.Sagas.First().SagaType;
            FromEndpoint = message.ReceivingEndpoint.Name;
            ToEndpoints = new[] { message.SendingEndpoint.Name };

            RefreshLastEndpointIndex();
        }

        public string Name { get; private set; }
        public string SagaName { get; set; }
        public bool IsStartMessage { get; set; }
        public string FromEndpoint { get; private set; }
        public IEnumerable<string> ToEndpoints { get; private set; }
        //public IEnumerable<string> TimeoutEndpoints { get; set; }
        public IEnumerable<EndpointInfo> Endpoints { get; set; }
        //public bool IsEvent { get; set; }
        public bool Selected { get; set; }

        public int MinEndpointIndex { get; private set; }
        public int MaxEndpointIndex { get; private set; }
        public IEnumerable<int> ToEndpointIndicies { get; private set; }
        //public IEnumerable<int> TimeoutEndpointIndicies { get; private set; }
        public int MessagePopupIndex { get; private set; }

        private int FindEndpointIndex(string endpointName)
        {
            var i = 0;
            foreach (var endpoint in Endpoints)
            {
                if (string.Equals(endpointName, endpoint.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
                i++;
            }
            return -1;
        }

        private void OnFromEndpointChanged()
        {
            RefreshLastEndpointIndex();
        }

        private void OnToEndpointsChanged()
        {
            ToEndpointIndicies = (ToEndpoints ?? Enumerable.Empty<string>()).Select(FindEndpointIndex).ToList();
            RefreshLastEndpointIndex();
        }

        //private void OnTimeoutEndpointsChanged()
        //{
        //    TimeoutEndpointIndicies = (TimeoutEndpoints ?? Enumerable.Empty<string>()).Select(FindEndpointIndex).ToList();
        //    RefreshLastEndpointIndex();
        //}

        private void RefreshLastEndpointIndex()
        {
            var endpointIndicies = new[] { FindEndpointIndex(FromEndpoint) }
                .Concat(ToEndpointIndicies ?? Enumerable.Empty<int>())
                //.Concat(TimeoutEndpointIndicies ?? Enumerable.Empty<int>())
                .ToList();

            MinEndpointIndex = endpointIndicies.Min();
            MaxEndpointIndex = endpointIndicies.Max();

            MessagePopupIndex = MinEndpointIndex + (MaxEndpointIndex - MinEndpointIndex) / 2;
        }
    }
}