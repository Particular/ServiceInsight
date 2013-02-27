using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.MessageViewers.JsonViewer
{
    public interface IJsonMessageViewModel :
        IViewAware,
        IScreen,
        IHandle<MessageBodyLoadedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        MessageBody SelectedMessage { get; set; }
    }
}