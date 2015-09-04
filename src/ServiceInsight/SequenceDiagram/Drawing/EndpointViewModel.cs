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

        public EndpointViewModel(Endpoint endpoint, string version)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint", "endpoint is null.");
            }

            FullName = Name = endpoint.Name;
           Version = version;
            Host = endpoint.Host ?? String.Empty;
        }

     
        public string FullName { get; protected set; }
        public string Version { get; protected set; }
        public string Host { get; protected set; }

        public override int GetHashCode()
        {
            return FullName.GetHashCode() ^ Host.GetHashCode() ^ Version.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as EndpointViewModel;
            if (other == null)
                return false;

            return string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }
    }
}