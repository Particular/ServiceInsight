namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    using Caliburn.PresentationFramework;

    public class MessageHeaderKeyValue : PropertyChangedBase
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}