using System;
using System.Collections.Generic;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Linq;
    using ReactiveUI;

    public class MessageInfo : ReactiveObject
    {
        public string Name { get; set; }
        public string SagaName { get; set; }
        public bool IsStartMessage { get; set; }
        public string FromEndpoint { get; set; }
        public IEnumerable<string> ToEndpoints { get; set; }
        public IEnumerable<string> TimeoutEndpoints { get; set; }
        public IEnumerable<EndpointInfo> Endpoints { get; set; }
        public bool IsEvent { get; set; }
        public bool Selected { get; set; }

        public int MinEndpointIndex { get; private set; }
        public int MaxEndpointIndex { get; private set; }
        public IEnumerable<int> ToEndpointIndicies { get; private set; }
        public IEnumerable<int> TimeoutEndpointIndicies { get; private set; }
        public int MessagePopupIndex { get; private set; }

        private int FindEndpointIndex(string endpointName)
        {
            var i = 0;
            foreach (var endpoint in Endpoints)
            {
                if (string.Equals(endpointName, endpoint.Name, StringComparison.InvariantCultureIgnoreCase))
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

        private void OnTimeoutEndpointsChanged()
        {
            TimeoutEndpointIndicies = (TimeoutEndpoints ?? Enumerable.Empty<string>()).Select(FindEndpointIndex).ToList();
            RefreshLastEndpointIndex();
        }

        private void RefreshLastEndpointIndex()
        {
            var endpointIndicies = new[] { FindEndpointIndex(FromEndpoint) }
                .Concat(ToEndpointIndicies ?? Enumerable.Empty<int>())
                .Concat(TimeoutEndpointIndicies ?? Enumerable.Empty<int>())
                .ToList();

            MinEndpointIndex = endpointIndicies.Min();
            MaxEndpointIndex = endpointIndicies.Max();

            MessagePopupIndex = MinEndpointIndex + (MaxEndpointIndex - MinEndpointIndex) / 2;
        }
    }
}