using Caliburn.PresentationFramework.Screens;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface IGeneralHeaderViewModel : IScreen
    {
        string Version { get; }
        string EnclosedMessageTypes { get; }
        string Retries { get; }
        string RelatedTo { get; }
        string ContentType { get; }
        string IsDeferedMessage { get; }
        string ConversationId { get; }
        string HeaderContent { get; }

        bool CanCopyHeaderInfo();
        void CopyHeaderInfo();
    }
}