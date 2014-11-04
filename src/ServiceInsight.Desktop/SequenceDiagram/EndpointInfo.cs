using System;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Diagnostics;
    using System.Linq;
    using Models;

    [DebuggerDisplay("{Name}")]
    public class EndpointInfo
    {
        public EndpointInfo(Endpoint endpoint, StoredMessage message)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "endpoint is null.");
            if (message == null)
                throw new ArgumentNullException("message", "message is null.");

            Name = string.Join(".", endpoint.Name.Split('.').Skip(1));
            if (string.IsNullOrEmpty(Name))
                Name = endpoint.Name;
            FullName = endpoint.Name;
            Version = message.GetHeaderByKey(MessageHeaderKeys.Version);
            Host = endpoint.Host;
        }

        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string Version { get; private set; }
        public string Host { get; private set; }

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
}