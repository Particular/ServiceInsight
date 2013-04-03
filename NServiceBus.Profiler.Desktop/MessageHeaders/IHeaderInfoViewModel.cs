using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IHeaderInfoViewModel : 
        IScreen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<MessageBodyLoaded>, 
        IHandle<SelectedMessageChanged>
    {
    }
}