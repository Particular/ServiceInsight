using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Plugins;

namespace NServiceBus.Profiler.Bus
{
    public interface IHeaderInfoViewModel : 
        IPlugin, 
        IHandle<MessageBodyLoadedEvent>, 
        IHandle<SelectedQueueChangedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        IObservableCollection<HeaderInfo> Items { get; }
        bool CanReturnToSource();
        void ReturnToSource();
    }
}