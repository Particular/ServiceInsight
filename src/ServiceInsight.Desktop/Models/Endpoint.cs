namespace Particular.ServiceInsight.Desktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Endpoint
    {
        public string Name { get; set; }

        public string HostDisplayName { get; set; }

        public List<EndpointProperty> EndpointProperties { get; set; }

        public bool EmitsHeartbeats
        {
            get
            {
                if (EndpointProperties == null) return false;
                var heartbeat = EndpointProperties.FirstOrDefault(p => string.Equals("monitored", p.Key, StringComparison.InvariantCultureIgnoreCase));
                if (heartbeat == null) return false;
                return bool.Parse(heartbeat.Value);
            }
        }

        public string Address
        {
            get { return string.Format("{0}{1}", Name, AtMachine()); }
        }

        string AtMachine()
        {
            return string.IsNullOrEmpty(HostDisplayName) ? string.Empty : string.Format("@{0}", HostDisplayName);
        }

        protected bool Equals(Endpoint other)
        {
            return string.Equals(Address, other.Address);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Endpoint) obj);
        }

        public override int GetHashCode()
        {
            return (Address != null ? Address.GetHashCode() : 0);
        }

        public static bool operator ==(Endpoint left, Endpoint right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Endpoint left, Endpoint right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return Address;
        }
    }

    [Serializable]
    public class EndpointProperty //not using KeyValuePair<,> as it is read-only and the rest client can't hyrate it
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}