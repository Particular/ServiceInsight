namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System.Threading.Tasks;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Events;
    using Explorer;
    using Models;
    using Search;
    using Shell;
    using Shell.Menu;

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
        MessageErrorInfo GetMessageErrorInfo(StoredMessage msg);
        void Focus(StoredMessage msg);
    }
}