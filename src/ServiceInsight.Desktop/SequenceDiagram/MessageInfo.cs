using System;
using System.Collections.Generic;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Linq;
    using ReactiveUI;

    public class MessageInfo : ReactiveObject
    {
        public string Name { get; set; }

        public bool IsStartMessage { get; set; }

        public string FromEndpoint { get; set; }

        public IEnumerable<string> ToEndpoints { get; set; }

        public IEnumerable<string> TimeoutEndpoints { get; set; }

        public IEnumerable<EndpointInfo> Endpoints { get; set; }

        public int FromEndpointIndex { get; private set; }

        public int LastToEndpointIndex { get; private set; }

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
            FromEndpointIndex = FindEndpointIndex(FromEndpoint);
        }

        private void OnToEndpointsChanged()
        {
            RefreshLastEndpointIndex();
            ToEndpointIndicies = ToEndpoints.Select(FindEndpointIndex).ToList();
        }

        private void OnTimeoutEndpointsChanged()
        {
            RefreshLastEndpointIndex();
            TimeoutEndpointIndicies = TimeoutEndpoints.Select(FindEndpointIndex).ToList();
        }

        private void RefreshLastEndpointIndex()
        {
            LastToEndpointIndex = ToEndpoints
                .Concat(TimeoutEndpoints ?? Enumerable.Empty<string>())
                .Select(FindEndpointIndex)
                .Aggregate((accum, next) => Math.Abs(next - FromEndpointIndex) > Math.Abs(accum - FromEndpointIndex) ? next : accum);

            if (LastToEndpointIndex > FromEndpointIndex)
                MessagePopupIndex = FromEndpointIndex;
            else
                MessagePopupIndex = LastToEndpointIndex;
        }
    }
}