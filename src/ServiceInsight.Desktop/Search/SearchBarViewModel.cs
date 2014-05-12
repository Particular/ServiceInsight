namespace Particular.ServiceInsight.Desktop.Search
{
    using System;
    using System.Collections.Generic;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.Filters;
    using Caliburn.PresentationFramework.Screens;
    using Core.Settings;
    using Events;
    using Explorer;
    using Explorer.EndpointExplorer;
    using Explorer.QueueExplorer;
    using ExtensionMethods;
    using MessageList;
    using Models;
    using Settings;
    using Startup;
	using System.Linq;

    public class SearchBarViewModel : Screen, ISearchBarViewModel
    {
        readonly ICommandLineArgParser commandLineArgParser;
        readonly ISettingsProvider settingProvider;
        int workCount;

        public SearchBarViewModel(ICommandLineArgParser commandLineArgParser, ISettingsProvider settingProvider)
        {
            this.commandLineArgParser = commandLineArgParser;
            this.settingProvider = settingProvider;
            PageSize = 50; //NOTE: Do we need to change this?
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            RestoreRecentSearchEntries();
            Search(commandLineArgParser.ParsedOptions.SearchQuery);
        }

        public void GoToFirstPage()
        {
            Parent.RefreshMessages(SelectedEndpoint, 1, SearchQuery);
        }

        public void GoToPreviousPage()
        {
            Parent.RefreshMessages(SelectedEndpoint, CurrentPage - 1, SearchQuery);
        }

        public void GoToNextPage()
        {
            Parent.RefreshMessages(SelectedEndpoint, CurrentPage + 1, SearchQuery);
        }

        public void GoToLastPage()
        {
            Parent.RefreshMessages(SelectedEndpoint, PageCount, SearchQuery);
        }

        public void Search(string searchQuery, bool performSearch = true)
        {
            SearchQuery = searchQuery;
            SearchInProgress = !SearchQuery.IsEmpty();
            SearchEnabled = !SearchQuery.IsEmpty();
            NotifyPropertiesChanged();

            if(performSearch) Search();
        }

        [AutoCheckAvailability]
        public async void Search()
        {
            SearchInProgress = true;
            AddRecentSearchEntry(SearchQuery);
            await Parent.RefreshMessages(SelectedEndpoint, 1, SearchQuery);
        }

        [AutoCheckAvailability]
        public async void CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
            await Parent.RefreshMessages(SelectedEndpoint, 1, SearchQuery);
        }

        public void SetupPaging(PagedResult<StoredMessage> pagedResult)
        {
            Result = pagedResult.Result;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;

            NotifyPropertiesChanged();
        }

        public async void RefreshResult()
        {
            await Parent.RefreshMessages(SelectedEndpoint, CurrentPage, SearchQuery);
        }

        public bool CanGoToLastPage
        {
            get
            {
                return CurrentPage < PageCount && !WorkInProgress;
            }
        }

        public bool CanCancelSearch
        {
            get { return SearchInProgress; }
        }

        public new IMessageListViewModel Parent
        {
            get { return base.Parent as IMessageListViewModel; }
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

        public bool WorkInProgress
        {
            get { return workCount > 0; }
        }

        public Endpoint SelectedEndpoint { get; private set; }

        public Queue SelectedQueue { get; private set; }
        
        public string SearchQuery { get; set; }

        public string SearchResultMessage
        {
            get { return GetSearchResultMessage(); }
        }

        public string SearchResultHeader
        {
            get { return GetSearchResultHeader(); }
        }

        public string SearchResultResults
        {
            get { return GetSearchResultResults(); }
        }

        public bool IsVisible { get; set; }

        public bool CanGoToFirstPage
        {
            get
            {
                return CurrentPage > 1 &&
                       !WorkInProgress;
            }
        }

        public bool CanGoToPreviousPage
        {
            get
            {
                return CurrentPage - 1 >= 1 &&
                       !WorkInProgress;
            }
        }

        public bool CanGoToNextPage
        {
            get
            {
                return CurrentPage + 1 <= PageCount &&
                       !WorkInProgress;
            }
        }

        public IList<StoredMessage> Result { get; private set; }

        public IObservableCollection<string> RecentSearchQueries { get; private set; }

        public int CurrentPage { get; private set; }
        
        public int PageSize { get; private set; }
        
        public int TotalItemCount { get; private set; }
        
        public bool SearchInProgress { get; private set; }
        
        public bool SearchEnabled { get; private set; }

        public bool CanSearch
        {
            get
            {
                return !WorkInProgress &&
                       !string.IsNullOrWhiteSpace(SearchQuery);
            }
        }

        public bool CanRefreshResult
        {
            get
            {
                return !WorkInProgress;
            }
        }

        public void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => PageCount);
            NotifyOfPropertyChange(() => CanGoToFirstPage);
            NotifyOfPropertyChange(() => CanGoToLastPage);
            NotifyOfPropertyChange(() => CanGoToNextPage);
            NotifyOfPropertyChange(() => CanGoToPreviousPage);
            NotifyOfPropertyChange(() => CanRefreshResult);
            NotifyOfPropertyChange(() => SearchEnabled);
            NotifyOfPropertyChange(() => CanCancelSearch);
            NotifyOfPropertyChange(() => WorkInProgress);
            NotifyOfPropertyChange(() => SearchResultMessage);
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

            var serviceNode = @event.SelectedExplorerItem.As<ServiceControlExplorerItem>();
            if (serviceNode != null)
            {
                SelectedEndpoint = null;
                SearchEnabled = true;
            }

            var queueNode = @event.SelectedExplorerItem.As<QueueExplorerItem>();
            if (queueNode != null)
            {
                SelectedQueue = queueNode.Queue;
            }

            NotifyPropertiesChanged();
        }

        public void Handle(WorkStarted @event)
        {
            workCount++;
            NotifyPropertiesChanged();
        }

        public void Handle(WorkFinished @event)
        {
            if (workCount > 0)
            {
                workCount--;
                NotifyPropertiesChanged();
            }
        }

        void RestoreRecentSearchEntries()
        {
            var setting = settingProvider.GetSettings<ProfilerSettings>();
            RecentSearchQueries = new BindableCollection<string>(setting.RecentSearchEntries);
        }

        void AddRecentSearchEntry(string searchQuery)
        {
            if (searchQuery.IsEmpty()) return;

            RecentSearchQueries.Add(searchQuery);

            var setting = settingProvider.GetSettings<ProfilerSettings>();
            if (!setting.RecentSearchEntries.Contains(searchQuery, StringComparer.OrdinalIgnoreCase))
            {
                setting.RecentSearchEntries.Add(searchQuery);
                settingProvider.SaveSettings(setting);
            }
        }

        string GetSearchResultMessage()
        {
            return string.Format("{0}{1}", GetSearchResultHeader(), GetSearchResultResults());
        }

        string GetSearchResultHeader()
        {
            if (SearchInProgress)
            {
                return "Search results:";
            }

            return SelectedEndpoint != null ?
                   SelectedEndpoint.Name : string.Empty;
        }

        string GetSearchResultResults()
        {
            if (SearchInProgress)
            {
                return SelectedEndpoint != null ?
                        string.Format(" {0} Message(s) found in Endpoint '{1}'", TotalItemCount, SelectedEndpoint.Name) :
                        string.Format(" {0} Message(s) found", TotalItemCount);
            }

            return string.Format(" {0} Message(s) found", TotalItemCount);
        }
    }
}