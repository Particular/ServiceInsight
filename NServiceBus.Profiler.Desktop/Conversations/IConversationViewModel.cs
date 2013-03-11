using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public interface IConversationViewModel : 
        IScreen, 
        IHandle<MessageBodyLoadedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        ConversationGraph Graph { get; set; }
    }
}