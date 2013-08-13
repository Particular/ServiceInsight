using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Events;

namespace Particular.ServiceInsight.Desktop.Conversations
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