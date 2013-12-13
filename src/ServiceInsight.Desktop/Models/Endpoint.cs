using System.Collections.Generic;
using System.Linq;

namespace NServiceBus.Profiler.Desktop.Models
{
    public class Endpoint
    {
        public string Name { get; set; }

        public string Machine
        {
            get
            {
                if (machine == null)
                {
                    if (Machines != null)
                    {
                        machine = Machines.FirstOrDefault();    
                    }
                    
                }

                return machine;
            }
            set
            {
                machine = value;
            }
        }

        private string machine;

        public List<string> Machines { get; set; }

        public string Address
        {
            get { return string.Format("{0}@{1}", Name, Machine); }
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
}