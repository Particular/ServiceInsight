namespace ServiceInsight.SequenceDiagram.Drawing
{
    using System;
    using System.Diagnostics;
    using Particular.ServiceInsight.Desktop.Models;

    [DebuggerDisplay("{Name}")]
    public class EndpointViewModel : UmlViewModel
    {
        protected EndpointViewModel()
        {
        }

        public EndpointViewModel(Endpoint endpoint, string version = null)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint", "endpoint is null.");
            }

            FullName = Title = endpoint.Name;
            Version = version;
            Host = endpoint.Host;
        }

        public string FullName { get; private set; }
        public string Version { get; private set; }
        public string Host { get; private set; }

        public override int GetHashCode()
        {
            return FullName.GetHashCode() ^ (Host ?? String.Empty).GetHashCode() ^ (Version ?? String.Empty).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as EndpointViewModel;
            if (other == null)
            {
                return false;
            }

            var firstPart = string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase);

            if (Version == null || other.Version == null)
            {
                return firstPart;
            }

            return firstPart && string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }
    }
}
