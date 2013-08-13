using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.MessageViewers.JsonViewer
{
    public interface IJsonMessageViewModel :
        IViewAware,
        IScreen,
        IHandle<MessageBodyLoaded>,
        IHandle<SelectedMessageChanged>
    {
        MessageBody SelectedMessage { get; set; }
    }
}