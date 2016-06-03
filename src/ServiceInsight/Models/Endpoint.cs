namespace ServiceInsight.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Endpoint
    {
        string host;

        public string Name { get; set; }

        public string Host
        {
            get { return host ?? HostDisplayName; }
            set { host = value; }
        }

        public string HostId { get; set; }

        public string HostDisplayName { get; set; }

        public List<EndpointProperty> EndpointProperties { get; set; }

        public bool EmitsHeartbeats
        {
            get
            {
                if (EndpointProperties == null)
                {
                    return false;
                }

                var heartbeat = EndpointProperties.FirstOrDefault(p => string.Equals("monitored", p.Key, StringComparison.InvariantCultureIgnoreCase));
                if (heartbeat == null)
                {
                    return false;
                }

                return bool.Parse(heartbeat.Value);
            }
        }

        public string Address => string.Format("{0}{1}", Name, AtMachine());

        string AtMachine() => string.IsNullOrEmpty(Host) ? string.Empty : string.Format("@{0}", Host);

        protected bool Equals(Endpoint other) => other != null && string.Equals(Address, other.Address);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
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

    [Serializable]
    public class EndpointProperty //not using KeyValuePair<,> as it is read-only and the rest client can't hydrate it
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}