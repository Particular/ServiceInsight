using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.MessageProperties
{
    public interface IHeaderInfoViewModel : 
        IScreen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<SelectedMessageChanged>
    {
    }
}