using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Linq;

namespace NServiceBus.Profiler.Common.Models
{
    ///<summary>
    /// Abstraction for an address on the NServiceBus network.
    ///</summary>
    [Serializable]
    public class Address : ISerializable, IComparable<Address>
    {
        internal const string DIRECTPREFIX = "DIRECT=OS:";
        internal const string DIRECTPREFIX_TCP = "DIRECT=TCP:";
        internal const string FORMATNAME_TCP = "FormatName:" + DIRECTPREFIX_TCP;
        internal const string FORMATNAME = "FormatName:" + DIRECTPREFIX;
        internal const string PRIVATE = "\\private$\\";

        private static readonly string DefaultMachine = Environment.MachineName;

        /// <summary>
        /// Get the address of this endpoint.
        /// </summary>
        public static Address Local { get; private set; }

        static Address()
        {
            Local = new Address(DefaultMachine, "");
        }

        public static Address ParseFormatName(string formatName)
        {
            var queueParts = formatName.Split('\\');

            if (queueParts.Length <= 1)
                throw new InvalidOperationException("Not a valid queue address.");

            var queueName = queueParts[queueParts.Length - 1];
            var queueType = formatName.Contains(PRIVATE) ? QueueTypes.Private : QueueTypes.Public;

            var directPrefixIndex = queueParts[0].IndexOf(DIRECTPREFIX, StringComparison.Ordinal);
            if (directPrefixIndex >= 0)
            {
                return new Address(queueParts[0].Substring(directPrefixIndex + DIRECTPREFIX.Length), queueName){ QueueType = queueType };
            }

            var tcpPrefixIndex = queueParts[0].IndexOf(DIRECTPREFIX_TCP, StringComparison.Ordinal);
            if (tcpPrefixIndex >= 0)
            {
                return new Address(queueParts[0].Substring(tcpPrefixIndex + DIRECTPREFIX_TCP.Length), queueName);
            }

            return new Address(queueParts[0], queueName);
        }

        /// <summary>
        /// Parses a string and returns an Address.
        /// </summary>
        public static Address Parse(string destination)
        {
            Guard.NotNullOrEmpty(() => destination, destination, "Invalid destination address specified");

            var part = destination.Split('@');
            var queue = part[0];
            var machine = DefaultMachine;

            Guard.NotOutOfRangeInclusive(() => part.Length, part.Length, 0, 2);

            if (part.Length == 2)
            {
                if (!IsLocal(part[1]))
                    machine = part[1];
            }

            return new Address(machine, queue) { QueueType = QueueTypes.Private };
        }

        public static bool IsLocal(string machine)
        {
            return machine == "." ||
                   machine.Equals(DefaultMachine, StringComparison.InvariantCultureIgnoreCase) ||
                   machine.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) ||
                   machine.Equals(IPAddress.Loopback.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool IsValidAddress(string address)
        {
            if (IsLocal(address)) return true;
            var ip = TryGetHostEntry(address);
            return ip != null;
        }

        static readonly Func<string, string> TryGetHostEntry = machine =>
        {
            if (string.IsNullOrWhiteSpace(machine)) 
                return null;

            try
            {
                var entry = Dns.GetHostEntry(machine);
                if (entry != null)
                {
                    var ipv4 = entry.AddressList
                                    .FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork);
                    if (ipv4 != null)
                    {
                        return ipv4.ToString().Trim();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        };

        public static bool IsRemote(string machineName)
        {
            if (IsLocal(machineName))
                return false;

            var isremote = true;

            var ip = TryGetHostEntry(machineName);
            var localIp = TryGetHostEntry(DefaultMachine);
            var name = machineName.Trim();
            var localName = DefaultMachine;

            if (localName.Equals(name, StringComparison.InvariantCultureIgnoreCase) ||
                (localIp.Equals(ip, StringComparison.InvariantCultureIgnoreCase)) ||
                (localIp.Equals(machineName, StringComparison.InvariantCultureIgnoreCase)))
            {
                isremote = false;
            }

            return isremote;
        }

        ///<summary>
        /// Instantiate a new Address for a known queue on a given machine.
        ///</summary>
        public Address(string machineName, string queueName)
        {
            Guard.NotNull(() => machineName, machineName);
            Guard.NotNull(() => queueName, queueName);

            Queue = queueName.ToLower();
            Machine = machineName.ToLower();
        }

        /// <summary>
        /// Instantiate a new Address for a known queue on a given machine.
        /// </summary>
        public Address(string queueName) : this(DefaultMachine, queueName)
        {
        }

        /// <summary>
        /// Deserializes an Address.
        /// </summary>
        protected Address(SerializationInfo info, StreamingContext context)
        {
            Queue = info.GetString("Queue");
            Machine = info.GetString("Machine");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Queue", Queue);
            info.AddValue("Machine", Machine);
        }

        /// <summary>
        /// Provides a hash code of the Address.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Queue != null ? Queue.GetHashCode() : 0) * 397) ^ (Machine != null ? Machine.GetHashCode() : 0);
            }
        }

        public int CompareTo(Address other)
        {
            return String.Compare(other.ToString(), ToString(), StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns a string representation of the address.
        /// </summary>
        public override string ToString()
        {
            return Queue + "@" + Machine;
        }

        /// <summary>
        /// Returns a format name for this address
        /// </summary>
        public string ToFormatName()
        {
            var formatNameConnection = IsRemote(Machine) ? FORMATNAME_TCP : FORMATNAME;
            return string.Format("{0}{1}{2}{3}", formatNameConnection, Machine, PRIVATE, Queue);
        }

        /// <summary>
        /// Returns a short format name for this address (excludes the connection type [direct/tcp]
        /// </summary>
        public string ToShortFormatName()
        {
            return string.Format("{0}{1}{2}", Machine, PRIVATE, Queue);
        }

        /// <summary>
        /// Returns path of this address (machine\\queue)
        /// </summary>
        public string ToPathName()
        {
            return string.Format("{0}\\{1}", Machine, Queue);
        }

        /// <summary>
        /// The (lowercase) name of the queue not including the name of the machine or location depending on the address mode.
        /// </summary>
        public string Queue { get; private set; }

        /// <summary>
        /// The (lowercase) name of the machine or the (normal) name of the location depending on the address mode.
        /// </summary>
        public string Machine { get; private set; }

        /// <summary>
        /// Address mode (public or private)
        /// </summary>
        public QueueTypes QueueType { get; private set; }

        /// <summary>
        /// Overloading for the == for the class Address
        /// </summary>
        public static bool operator ==(Address left, Address right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Overloading for the != for the class Address
        /// </summary>
        public static bool operator !=(Address left, Address right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            var address = obj as Address;
            if (address != null)
            {
                return Equals(address);
            }

            var path = obj as string;
            if (path != null)
            {
                return Equals(Parse(path));
            }
            
            return false;
        }

        /// <summary>
        /// Check this is equal to other Address
        /// </summary>
        public bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Queue, Queue) && Equals(other.Machine, Machine);
        }

        /// <summary>
        /// Returns an address that would be either an IPV4 or the machine name.
        /// </summary>
        public static string GetIpAddressOrMachineName(string machineName)
        {
            Guard.True(IsValidAddress(machineName));

            if (IsRemote(machineName))
            {
                var ipAddress = Dns.GetHostAddresses(machineName)
                                   .First(ip => ip.AddressFamily == AddressFamily.InterNetwork);

                return ipAddress.ToString();
            }
            
            return Local.Machine;
        }
    }
}