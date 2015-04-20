namespace Particular.ServiceInsight.Desktop.Models
{
    public class HeaderInfo
    {
        const string Null = "<Null>";
        public const string FailedQueueKey = "NServiceBus.FailedQ";
        public const string MessageDateFormat = "yyyy-MM-dd HH:mm:ss:ffffff Z";

        public string Key { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0}: {1}", GetDisplayKey(), GetDisplayValue());
        }

        string GetDisplayValue()
        {
            return Value ?? Null;
        }

        string GetDisplayKey()
        {
            if (Key == null)
                return Null;

            return Key.StartsWith("NServiceBus.") ? Key.Substring(12) : Key;
        }
    }
}