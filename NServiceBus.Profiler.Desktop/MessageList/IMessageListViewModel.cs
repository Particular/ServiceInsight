using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Explorer;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Desktop.Search;
using Particular.ServiceInsight.Desktop.Shell;

namespace Particular.ServiceInsight.Desktop.MessageList
{
    public interface IMessageListViewModel : IScreen, 
        IWorkTracker,
        IHandle<MessageRemovedFromQueue>,
        IHandle<AutoRefreshBeat>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailedEvent>
    {
        IObservableCollection<MessageInfo> Messages { get; }
        IObservableCollection<MessageInfo> SelectedMessages { get; }
        IObservableCollection<ContextMenuModel> ContextMenuItems { get; }
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