namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface IGeneralHeaderViewModel : IPropertyDataProvider
    {
        string Version { get; }
        string EnclosedMessageTypes { get; }
        string RelatedTo { get; }
        string ContentType { get; }
        string IsDeferedMessage { get; }
        string ConversationId { get; }
        string HeaderContent { get; }
    }
}