using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Search;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IMessageListViewModel : IScreen, 
        IHandle<MessageRemovedFromQueue>,
        IHandle<AutoRefreshBeat>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>
    {
        IObservableCollection<MessageInfo> Messages { get; }
        IObservableCollection<MessageInfo> SelectedMessages { get; }
        ISearchBarViewModel SearchBar { get; }
        ExplorerItem SelectedExplorerItem { get; }
        MessageInfo FocusedMessage { get; set; }
        Queue SelectedQueue { get; }
        Task PurgeQueue();
        Task DeleteSelectedMessages();
        Task RefreshMessages();
        Task RefreshQueue(Queue queue);
        Task RefreshEndpoint(Endpoint endpoint, int pageIndex = 1, string searchQuery = null);
        string GetCriticalTime(StoredMessage msg);
    }
}