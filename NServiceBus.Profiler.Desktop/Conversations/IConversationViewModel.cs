using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.Conversations
{
    public interface IConversationViewModel : 
        IScreen, 
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        ConversationGraph Graph { get; set; }
        void GraphLayoutUpdated();
    }
}