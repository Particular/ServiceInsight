using System.Windows.Media;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Events;

namespace NServiceBus.Profiler.Desktop.MessageHeaders
{
    public interface IHeaderInfoViewModel : 
        IScreen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<MessageBodyLoaded>, 
        IHandle<SelectedMessageChanged>
    {
        IObservableCollection<HeaderInfo> Items { get; }

        ImageSource GroupImage { get; }

        TabPriority Order { get; }
    }
}