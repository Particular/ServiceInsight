using System.Threading.Tasks;
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
        ITableViewModel<StoredMessage>,
        IWorkTracker,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>,
        IHandle<MessageStatusChanged>,
        IHandle<BodyTabSelectionChanged>
    {
        ISearchBarViewModel SearchBar { get; }
        ExplorerItem SelectedExplorerItem { get; }
        Task RefreshMessages(string orderBy = null, bool ascending = false);
        Task RefreshMessages(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false);
        string GetCriticalTime(StoredMessage msg);
        string GetProcessingTime(StoredMessage msg);
        string GetDeliveryTime(StoredMessage msg);
        MessageErrorInfo GetMessageErrorInfo(StoredMessage msg);
        MessageErrorInfo GetMessageErrorInfo();
    }
}