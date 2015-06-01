using System;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Diagnostics;
    using System.Linq;
    using Models;

    [DebuggerDisplay("{Name}")]
    public class EndpointInfo
    {
        protected EndpointInfo()
        {
        }

        public EndpointInfo(Endpoint endpoint, string version)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "endpoint is null.");
            
            Name = string.Join(".", endpoint.Name.Split('.').Skip(1));
            if (string.IsNullOrEmpty(Name))
            {
                Name = endpoint.Name;
            }
            FullName = endpoint.Name;
            Version = version;
            Host = endpoint.Host ?? "";
        }

        public string Name { get; protected set; }
        public string FullName { get; protected set; }
        public string Version { get; protected set; }
        public string Host { get; protected set; }

        public override int GetHashCode()
        {
            return FullName.GetHashCode() ^ Host.GetHashCode() ^ Version.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as EndpointInfo;
            if (other == null)
                return false;

            return string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase) && 
                string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class MessageEndpointInfo : EndpointInfo
    {
        public MessageEndpointInfo(EndpointInfo endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "endpoint is null.");

            Name = endpoint.Name;
            FullName = endpoint.FullName;
            Version = endpoint.Version;
            Host = endpoint.Host;
        }

        public bool IsMessageLine { get; set; }
        public bool IsPublished { get; set; }
    }
}