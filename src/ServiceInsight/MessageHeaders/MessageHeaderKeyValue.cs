namespace ServiceInsight.MessageHeaders
{
    using Pirac;

    public class MessageHeaderKeyValue : BindableObject
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}