using System;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;

namespace NServiceBus.Profiler.Desktop.Search
{
    public class SearchBarViewModel : Screen, ISearchBarViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        
        public SearchBarViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            PageSize = 50; //NOTE: Do we need to change this?
        }

        public virtual Endpoint SelectedEndpoint { get; set; }

        public void GoToFirstPage()
        {
            _eventAggregator.Publish(new LoadAuditMessages
            {
                Endpoint = SelectedEndpoint,
                PageIndex = 1,
                SearchQuery = SearchQuery,
            });
        }

        public bool CanGoToFirstPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage > 1;
            }
        }

        public void GoToPreviousPage()
        {
            _eventAggregator.Publish(new LoadAuditMessages
            {
                Endpoint = SelectedEndpoint,
                PageIndex = CurrentPage - 1,
                SearchQuery = SearchQuery,
            });
        }

        public bool CanGoToPreviousPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage - 1 >= 1;
            }
        }
        
        public void GoToNextPage()
        {
            _eventAggregator.Publish(new LoadAuditMessages
            {
                Endpoint = SelectedEndpoint,
                PageIndex = CurrentPage + 1,
                SearchQuery = SearchQuery,
            });
        }

        public bool CanGoToNextPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage + 1 <= PageCount;
            }
        }

        public void GoToLastPage()
        {
            _eventAggregator.Publish(new LoadAuditMessages
            {
                Endpoint = SelectedEndpoint,
                PageIndex = PageCount,
                SearchQuery = SearchQuery,
            });
        }

        public bool CanGoToLastPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage < PageCount;
            }
        }

        public int CurrentPage
        {
            get; private set;
        }

        public int PageSize
        {
            get; private set;
        }

        public int TotalItemCount
        {
            get; private set;
        }

        public int PageCount
        {
            get
            {
                if (TotalItemCount == 0)
                {
                    return 0;
                }

                return (int)Math.Ceiling((double)TotalItemCount / PageSize);
            }
        }

        public virtual bool SearchInProgress { get; set; }

        public virtual bool SearchEnabled { get; private set; }

        public virtual bool CanSearch()
        {
            return SelectedEndpoint != null &&
                   !string.IsNullOrWhiteSpace(SearchQuery);
        }

        public virtual bool CanCancelSearch()
        {
            return SearchInProgress;
        }

        [AutoCheckAvailability]
        public virtual void Search()
        {
            SearchInProgress = true;
            _eventAggregator.Publish(new LoadAuditMessages
            {
                Endpoint = SelectedEndpoint,
                PageIndex = 1,
                SearchQuery = SearchQuery,
            });
        }

        [AutoCheckAvailability]
        public virtual void CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
        }

        public bool CanRefreshResult
        {
            get
            {
                return SelectedEndpoint != null ||
                       (SearchInProgress && SearchQuery != null);
            }
        }

        public void RefreshResult()
        {
            
        }

        public PagedResult<StoredMessage> Result
        {
            get; private set;
        }

        public void SetupPaging(PagedResult<StoredMessage> pagedResult)
        {
            Result = pagedResult;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;

            NotifyOfPropertyChange(() => PageCount);
            NotifyOfPropertyChange(() => CanGoToFirstPage);
            NotifyOfPropertyChange(() => CanGoToLastPage);
            NotifyOfPropertyChange(() => CanGoToNextPage);
            NotifyOfPropertyChange(() => CanGoToPreviousPage);
            NotifyOfPropertyChange(() => CanRefreshResult);
            NotifyOfPropertyChange(() => SearchEnabled);
        }

        public virtual string SearchQuery { get; set; }

        public virtual void Handle(EndpointSelectionChanged @event)
        {
            SearchEnabled = true;
            SelectedEndpoint = @event.SelectedEndpoint;
        }

        public virtual void Handle(SelectedQueueChanged @event)
        {
            SearchEnabled = false;
        }
    }

    public interface ISearchBarViewModel : 
        IHandle<EndpointSelectionChanged>
    {
        Endpoint SelectedEndpoint { get; }
        string SearchQuery { get; }
        void GoToFirstPage();
        void GoToLastPage();
        void GoToPreviousPage();
        void GoToNextPage();
        void Search();
        void CancelSearch();
        void SetupPaging(PagedResult<StoredMessage> pagedResult);
    }
}