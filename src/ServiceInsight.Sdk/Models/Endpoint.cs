namespace ServiceInsight.Models
{
    public class Endpoint
    {
        public string Name
        {
            get => EndpointDetails.Name;
            set => EndpointDetails.Name = value;
        }

        public string Host
        {
            get => EndpointDetails.Host ?? HostDisplayName;
            set => EndpointDetails.Host = value;
        }

        public string HostId
        {
            get => EndpointDetails.HostId;
            set => EndpointDetails.HostId = value;
        }

        public string HostDisplayName { get; set; }

        public string Address => $"{Name}{AtMachine()}";

        public EndpointDetails EndpointDetails { get; set; } = new EndpointDetails();

        string AtMachine() => string.IsNullOrEmpty(Host) ? string.Empty : $"@{Host}";

        protected bool Equals(Endpoint other) => other != null && string.Equals(Address, other.Address);

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

            return Equals((Endpoint)obj);
        }

        public override int GetHashCode() => Address != null ? Address.GetHashCode() : 0;

        public static bool operator ==(Endpoint left, Endpoint right) => Equals(left, right);

        public static bool operator !=(Endpoint left, Endpoint right) => !Equals(left, right);

        public override string ToString() => Address;
    }

    public class EndpointDetails
    {
        public string Name { get; set; }

        public string HostId { get; set; }

        public string Host { get; set; }
    }
}