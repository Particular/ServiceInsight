using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using Particular.ServiceInsight.Desktop.Events;
using Particular.ServiceInsight.Desktop.Models;
using Particular.ServiceInsight.Desktop.Shell;

namespace Particular.ServiceInsight.Desktop.Search
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
        bool IsVisible { get; set; }
        void GoToFirstPage();
        void GoToLastPage();
        void GoToPreviousPage();
        void GoToNextPage();
        void Search();
        void CancelSearch();
        void SetupPaging(PagedResult<MessageInfo> pagedResult);
        void NotifyPropertiesChanged();
    }
}