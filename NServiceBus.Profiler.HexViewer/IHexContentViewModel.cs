using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Views;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.HexViewer
{
    public interface IHexContentViewModel : 
        IPlugin, 
        IViewAware,
        IHandle<MessageBodyLoadedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        byte[] CurrentContent { get; set; }
        IObservableCollection<HexPart> HexParts { get; }
    }
}