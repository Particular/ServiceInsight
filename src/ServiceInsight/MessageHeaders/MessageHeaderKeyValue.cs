namespace ServiceInsight.MessageHeaders
{
    using Caliburn.Micro;

    public class MessageHeaderKeyValue : PropertyChangedBase
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}