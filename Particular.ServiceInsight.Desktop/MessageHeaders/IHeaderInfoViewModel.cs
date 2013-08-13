using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Events;

namespace Particular.ServiceInsight.Desktop.MessageHeaders
{
    public interface IHeaderInfoViewModel : 
        IScreen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<MessageBodyLoaded>, 
        IHandle<SelectedMessageChanged>
    {
    }
}