namespace ServiceInsight.SequenceDiagram.Drawing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Title}")]
    public class EndpointViewModel : UmlViewModel, IEquatable<EndpointViewModel>
    {
        protected EndpointViewModel()
        {
        }

        public EndpointViewModel(string name, string host, string version = null)
        {
            FullName = Title = name;
            Version = version;
            Host = host;

            Handlers = new List<HandlerViewModel>();
        }

        public string FullName { get; private set; }
        public string Version { get; private set; }
        public string Host { get; private set; }
        public List<HandlerViewModel> Handlers { get; private set; }

        public bool Equals(EndpointViewModel other)
        {
            var firstPart = string.Equals(FullName, other.FullName, StringComparison.OrdinalIgnoreCase) &&
                     string.Equals(Host, other.Host, StringComparison.OrdinalIgnoreCase);

            if (Version == null || other.Version == null)
            {
                return firstPart;
            }

            return firstPart && string.Equals(Version, other.Version, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            var other = obj as EndpointViewModel;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FullName != null ? FullName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Version != null ? Version.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Host != null ? Host.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
