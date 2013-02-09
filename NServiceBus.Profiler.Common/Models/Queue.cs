using System;
using Caliburn.PresentationFramework;

namespace NServiceBus.Profiler.Common.Models
{
    [Serializable]
    public class Queue : PropertyChangedBase, IComparable<Queue>
    {
        public Queue(string queueName) : this(new Address(queueName))
        {
            Address = new Address(queueName);
        }

        public Queue(string machineName, string queueName) : this(new Address(machineName, queueName))
        {
        }

        public Queue(Address address)
        {
            Address = address;
            FormatName = address.ToFormatName();
        }

        public Address Address { get; private set; }
        public string FormatName { get; set; }
        public QueueTypes QueueType { get; set; }
        public bool IsTransactional { get; set; }
        public bool CanRead { get; set; }

        public bool IsRemoteQueue()
        {
            Guard.NotNull(() => Address, Address);
            return Address.IsRemote(Address.Machine);
        }

        public int CompareTo(Queue other)
        {
            return other.Address.CompareTo(Address);
        }

        public override string ToString()
        {
            return Address.ToString();
        }
    }
}