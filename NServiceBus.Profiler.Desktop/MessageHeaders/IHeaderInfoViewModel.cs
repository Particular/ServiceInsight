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
        IHandle<MessageBodyLoaded>, 
        IHandle<SelectedQueueChanged>,
        IHandle<SelectedMessageChanged>
    {
        IObservableCollection<HeaderInfo> Items { get; }

        ImageSource GroupImage { get; }

        TabPriority Order { get; }
    }
}