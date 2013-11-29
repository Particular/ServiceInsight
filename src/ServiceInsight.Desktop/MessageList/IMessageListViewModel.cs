using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Shell.Menu;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public interface IMessageListViewModel : IScreen, 
        IHaveContextMenu,
        IWorkTracker,
        IHandle<MessageRemovedFromQueue>,
        IHandle<EndpointAutoRefreshBeat>,
        IHandle<QueueAutoRefreshBeat>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailedEvent>,
        IHandle<MessageStatusChanged>
    {
        IObservableCollection<MessageInfo> Messages { get; }
        IObservableCollection<MessageInfo> SelectedMessages { get; }
        ISearchBarViewModel SearchBar { get; }
        ExplorerItem SelectedExplorerItem { get; }
        MessageInfo FocusedMessage { get; set; }
        Queue SelectedQueue { get; }
        Task PurgeQueue();
        Task DeleteSelectedMessages();
        Task RefreshMessages(string columnName = null, bool ascending = false);
        Task RefreshQueue(Queue queue);
        Task RefreshEndpoint(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false);
        string GetCriticalTime(StoredMessage msg);
        string GetProcessingTime(StoredMessage msg);
        MessageErrorInfo GetMessageErrorInfo(StoredMessage msg);
        MessageErrorInfo GetMessageErrorInfo();
    }
}