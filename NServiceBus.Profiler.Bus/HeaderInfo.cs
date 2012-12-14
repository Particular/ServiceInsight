namespace NServiceBus.Profiler.Bus
{
    public class HeaderInfo
    {
        public const string FailedQueueKey = "NServiceBus.FailedQ";

        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("Key={0}, Value={1}", Key, Value ?? "<Null>");
        }
    }
}