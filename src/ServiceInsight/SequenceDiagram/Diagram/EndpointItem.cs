namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("{Name}")]
    public class EndpointItem : DiagramItem, IEquatable<EndpointItem>
    {
        HashSet<EndpointHost> hosts;

        public EndpointItem(string name, string host, string id, string version = null)
        {
            FullName = Name = name;

            hosts = new HashSet<EndpointHost>
            {
                new EndpointHost(host, id, version)
            };

            Timeline = new EndpointTimeline
            {
                Endpoint = this
            };

            Handlers = new List<Handler>();
        }

        public EndpointTimeline Timeline { get; }

        public string FullName { get; }

        public IReadOnlyList<EndpointHost> Hosts => hosts.ToList();

        public string HostId => string.Join(",", Hosts.Select(h => h.HostId));

        public string Host => string.Join(",", Hosts.Select(h => h.Host));

        public string Version => GetVersions();

        string GetVersions()
        {
            var versions = string.Join(",", Hosts.Select(h => h.Versions));
            return string.IsNullOrEmpty(versions) ? null : versions;
        }

        public List<Handler> Handlers { get; }

        public void AddHost(EndpointHost host)
        {
            if (!Hosts.Contains(host))
            {
                hosts.Add(host);
            }
            else
            {
                var existingHost = FindHost(host.HostId, host.Host);
                existingHost.AddHostVersions(host.HostVersions);
            }
        }

        public EndpointHost FindHost(string hostId, string hostName)
        {
            return hosts.SingleOrDefault(h => h.HostId == hostId && h.Host == hostName);
        }

        public bool Equals(EndpointItem other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return FullName == other.FullName;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((EndpointItem)obj);
        }

        public override int GetHashCode()
        {
            return FullName != null ? FullName.GetHashCode() : 0;
        }
    }

    [DebuggerDisplay("{Host}")]
    public class EndpointHost : IEquatable<EndpointHost>
    {
        HashSet<string> hostVersions;

        public EndpointHost(string host, string hostId, string version = null)
        {
            Host = host;
            HostId = hostId;
            hostVersions = new HashSet<string>();
            if (!string.IsNullOrEmpty(version))
            {
                hostVersions.Add(version);
            }
        }

        public string Host { get; }

        public string HostId { get; }

        public string Versions => HostVersions.Count == 0 ? null : string.Join(", ", HostVersions);

        public IReadOnlyList<string> HostVersions => hostVersions.ToList();

        public bool Equals(EndpointHost other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return HostId == other.HostId && Host == other.Host;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((EndpointHost)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Host != null ? Host.GetHashCode() : 0) * 397) ^ (HostId != null ? HostId.GetHashCode() : 0);
            }
        }

        public void AddHostVersions(IEnumerable<string> versions)
        {
            foreach (var version in versions)
            {
                if (!string.IsNullOrEmpty(version) && !hostVersions.Contains(version, StringComparer.InvariantCultureIgnoreCase))
                {
                    hostVersions.Add(version);
                }
            }
        }
    }
}
