using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop.Search
{
    public interface ISearchBarViewModel : 
        IScreen,
        IChild,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IWorkTracker
    {
        Endpoint SelectedEndpoint { get; }
        string SearchQuery { get; }
        bool SearchEnabled { get; }
        bool IsVisible { get; set; }
        void GoToFirstPage();
        void GoToLastPage();
        void GoToPreviousPage();
        void GoToNextPage();
        void Search();
        void CancelSearch();
        void SetupPaging(PagedResult<StoredMessage> pagedResult);
        void NotifyPropertiesChanged();
    }
}