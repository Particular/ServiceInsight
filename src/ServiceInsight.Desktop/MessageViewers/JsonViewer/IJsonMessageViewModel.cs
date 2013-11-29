using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer
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