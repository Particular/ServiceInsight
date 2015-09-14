namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Name}")]
    public class EndpointItem : DiagramItem, IEquatable<EndpointItem>
    {
        public EndpointItem(string name, string host, string id, string version = null)
        {
            HostId = id;
            FullName = Name = name;
            Version = version;
            Host = host;

            Timeline = new EndpointTimeline
            {
                Endpoint = this,
            };
            Handlers = new List<Handler>();
        }

        public EndpointTimeline Timeline { get; set; }

        public string FullName { get; private set; }
        public string Version { get; private set; }
        public string Host { get; private set; }
        public string HostId { get; private set; }

        public List<Handler> Handlers { get; private set; }

        public override int GetHashCode()
        {
            return FullName.GetHashCode() ^ (HostId ?? String.Empty).GetHashCode() ^ (Host ?? String.Empty).GetHashCode() ^ (Version ?? String.Empty).GetHashCode();
        }

        public bool Equals(EndpointItem other)
        {
            var firstPart = string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase) &&
                            string.Equals(HostId, other.HostId, StringComparison.OrdinalIgnoreCase);

            if (Version == null || other.Version == null)
            {
                return firstPart;
            }

            return firstPart && string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            var other = obj as EndpointItem;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }
    }
}
