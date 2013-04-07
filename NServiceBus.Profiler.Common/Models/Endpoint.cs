namespace NServiceBus.Profiler.Common.Models
{
    public class Endpoint
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Machine { get; set; }

        public string Address
        {
            get { return string.Format("{0}@{1}", Name, Machine); }
        }

        public override string ToString()
        {
            return Address;
        }
    }
}