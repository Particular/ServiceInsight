using System.Windows.Media;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IHeaderInfoViewModel : 
        IScreen,
        IHandle<MessageBodyLoadedEvent>, 
        IHandle<SelectedQueueChangedEvent>,
        IHandle<SelectedMessageChangedEvent>
    {
        IObservableCollection<HeaderInfo> Items { get; }

        ImageSource GroupImage { get; }
    }
}