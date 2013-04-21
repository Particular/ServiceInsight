namespace NServiceBus.Profiler.Common.Models
{
    public class HeaderInfo
    {
        private const string Null = "<Null>";
        public const string FailedQueueKey = "NServiceBus.FailedQ";
        public const string MessageDateFormat = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", GetDisplayKey(), GetDisplayValue());
        }

        private string GetDisplayValue()
        {
            return Value ?? Null;
        }

        private string GetDisplayKey()
        {
            if (Key == null)
                return Null;

            return Key.StartsWith("NServiceBus.") ? Key.Substring(12) : Key;
        }
    }
}