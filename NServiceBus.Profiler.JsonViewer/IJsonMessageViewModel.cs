using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.JsonViewer
{
    public interface IJsonMessageViewModel :
        IPlugin,
        IViewAware,
        IHandle<MessageBodyLoadedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        MessageBody SelectedMessage { get; set; }
    }
}