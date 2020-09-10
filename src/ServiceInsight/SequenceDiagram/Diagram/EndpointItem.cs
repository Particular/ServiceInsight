namespace ServiceInsight.SequenceDiagram.Diagram
{
    using Comparers.Linq;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{Name}")]
    public class EndpointItem : DiagramItem, IEquatable<EndpointItem>
    {
        public EndpointItem(string name, string host, string id, string version = null)
        {
            FullName = Name = name;
            // add to a list of hosts
            // TODO: extract to a method
            var newhost = new EndpointHost(host, id, version);

            if (Hosts == null)
            {
                Hosts = new List<EndpointHost>();
                Hosts.Append(newhost);
            }
            
            if (!Hosts.Contains<EndpointHost>(newhost))
            {
                Hosts.Append(newhost);
            }

            Timeline = new EndpointTimeline
            {
                Endpoint = this,
            };
            Handlers = new List<Handler>();
        }

        public EndpointTimeline Timeline { get; }

        public string FullName { get; }

        public IEnumerable<EndpointHost> Hosts { get; }

        // TODO: toString and add commas to the id list
        public string HostIdList => string.Join(",", Hosts.Select(a => a.Host));
        // TODO: toString and add commas to the host names list
        public string Hostlist =>  string.Join(",", Hosts.Select(a => a.HostId));

        public List<Handler> Handlers { get; }
        public string Version { get; set; }

        public override int GetHashCode() => FullName.GetHashCode() ^ (HostIdList ?? string.Empty).GetHashCode() ^ (Hostlist ?? string.Empty).GetHashCode() ^ (Version ?? string.Empty).GetHashCode();

        public bool Equals(EndpointItem other)
        {
            var firstPart = string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);

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

    public class EndpointHost : IEquatable<EndpointHost>
    {
        public EndpointHost(string host, string hostId, string version = null)
        {
            Host = host;
            HostId = hostId;
            Version = version;
        }

        public string Host { get; }

        public string HostId { get; }

        public string Version { get; }

        public bool Equals(EndpointHost other)
        {
            var firstPart = string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(HostId, other.HostId, StringComparison.OrdinalIgnoreCase);

            if (Version == null || other.Version == null)
            {
                return firstPart;
            }

            return firstPart && string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() => Host.GetHashCode() ^ (HostId ?? string.Empty).GetHashCode() ^ (Host ?? string.Empty).GetHashCode() ^ (Version ?? string.Empty).GetHashCode();
    }
}
