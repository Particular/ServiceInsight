using System;
using System.Collections.Generic;
using Caliburn.PresentationFramework.Filters;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.MessageList;
using NServiceBus.Profiler.Desktop.Explorer;

namespace NServiceBus.Profiler.Desktop.Search
{
    public class SearchBarViewModel : Screen, ISearchBarViewModel
    {
        private int _workCount = 0;

        public SearchBarViewModel()
        {
            PageSize = 50; //NOTE: Do we need to change this?
        }

        public virtual void GoToFirstPage()
        {
            Parent.RefreshEndpoint(SelectedEndpoint, 1, SearchQuery);
        }

        public virtual void GoToPreviousPage()
        {
            Parent.RefreshEndpoint(SelectedEndpoint, CurrentPage - 1, SearchQuery);
        }

        public virtual void GoToNextPage()
        {
            Parent.RefreshEndpoint(SelectedEndpoint, CurrentPage + 1, SearchQuery);
        }

        public virtual void GoToLastPage()
        {
            Parent.RefreshEndpoint(SelectedEndpoint, PageCount, SearchQuery);
        }

        [AutoCheckAvailability]
        public virtual async void Search()
        {
            SearchInProgress = true;
            await Parent.RefreshEndpoint(SelectedEndpoint, 1, SearchQuery);
        }

        [AutoCheckAvailability]
        public virtual void CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
        }

        public void SetupPaging(PagedResult<MessageInfo> pagedResult)
        {
            Result = pagedResult.Result;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;

            NotifyPropertiesChanged();
        }

        public async void RefreshResult()
        {
            if (SelectedEndpoint != null)
            {
                await Parent.RefreshEndpoint(SelectedEndpoint, CurrentPage, SearchQuery);
            }
            else
            {
                await Parent.RefreshMessages();
            }
        }

        public virtual bool CanGoToLastPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage < PageCount &&
                       !WorkInProgress;
            }
        }

        public virtual bool CanCancelSearch
        {
            get { return SearchInProgress; }
        }

        public new IMessageListViewModel Parent
        {
            get { return base.Parent as IMessageListViewModel; }
        }
        
        public virtual int PageCount
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

        public virtual bool WorkInProgress
        {
            get { return _workCount > 0; }
        }

        public virtual Endpoint SelectedEndpoint { get; private set; }

        public virtual Queue SelectedQueue { get; private set; }
        
        public virtual string SearchQuery { get; set; }

        public virtual bool IsVisible { get; set; }

        public virtual bool CanGoToFirstPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage > 1 &&
                       !WorkInProgress;
            }
        }

        public virtual bool CanGoToPreviousPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage - 1 >= 1 &&
                       !WorkInProgress;
            }
        }

        public virtual bool CanGoToNextPage
        {
            get
            {
                return SelectedEndpoint != null &&
                       CurrentPage + 1 <= PageCount &&
                       !WorkInProgress;
            }
        }

        public virtual IList<MessageInfo> Result { get; private set; }

        public virtual int CurrentPage { get; private set; }
        
        public virtual int PageSize { get; private set; }
        
        public virtual int TotalItemCount { get; private set; }
        
        public virtual bool SearchInProgress { get; private set; }
        
        public virtual bool SearchEnabled { get; private set; }

        public virtual bool CanSearch
        {
            get
            {
                return !WorkInProgress &&
                       !string.IsNullOrWhiteSpace(SearchQuery) &&
                       SelectedEndpoint != null;
            }
        }

        public bool CanRefreshResult
        {
            get
            {
                return !WorkInProgress && (SelectedEndpoint != null || SelectedQueue != null);
            }
        }

        private void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => PageCount);
            NotifyOfPropertyChange(() => CanGoToFirstPage);
            NotifyOfPropertyChange(() => CanGoToLastPage);
            NotifyOfPropertyChange(() => CanGoToNextPage);
            NotifyOfPropertyChange(() => CanGoToPreviousPage);
            NotifyOfPropertyChange(() => CanRefreshResult);
            NotifyOfPropertyChange(() => SearchEnabled);
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public void OnSelectedEndpointChanged()
        {
            if (SelectedEndpoint != null)
            {
                SelectedQueue = null;
                SearchEnabled = true;
            }
        }

        public void OnSelectedQueueChanged()
        {
            if (SelectedQueue != null)
            {
                SelectedEndpoint = null;
                SearchEnabled = false;
            }
        }

        public virtual void Handle(SelectedExplorerItemChanged @event)
        {
            var endpointNode = @event.SelectedExplorerItem.As<EndpointExplorerItem>();
            if (endpointNode != null)
            {
                SelectedEndpoint = endpointNode.Endpoint;                
            }

            var queueNode = @event.SelectedExplorerItem.As<QueueExplorerItem>();
            if (queueNode != null)
            {
                SelectedQueue = queueNode.Queue;
            }

            NotifyPropertiesChanged();
        }

        public virtual void Handle(WorkStarted @event)
        {
            _workCount++;
            NotifyPropertiesChanged();
        }

        public virtual void Handle(WorkFinished @event)
        {
            if (_workCount > 0)
            {
                _workCount--;
                NotifyPropertiesChanged();
            }
        }
    }
}