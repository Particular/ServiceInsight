using System;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Diagnostics;
    using System.Linq;
    using Models;

    [DebuggerDisplay("{Name}")]
    public class EndpointInfo
    {
        public EndpointInfo(Endpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "endpoint is null.");

            Name = string.Join(".", endpoint.Name.Split('.').Skip(1));
            FullName = endpoint.Name;
        }

        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string Version { get { return "UNKNOWN"; } }
        public string Host { get { return "UNKNOWN"; } }
        public string Active { get { return "UNKNOWN"; } }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as EndpointInfo;
            if (other != null)
                return Name.Equals(other.Name);

            return false;
        }
    }
}