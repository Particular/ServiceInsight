namespace Particular.ServiceInsight.Desktop.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Core.Settings;
    using Events;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using MessageList;
    using Models;
    using Settings;
    using Shell;
    using Startup;

    public class SearchBarViewModel : Screen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IWorkTracker
    {
        private const int MAX_SAVED_SEARCHES = 10;
        CommandLineArgParser commandLineArgParser;
        ISettingsProvider settingProvider;
        int workCount;

        public SearchBarViewModel(CommandLineArgParser commandLineArgParser, ISettingsProvider settingProvider)
        {
            this.commandLineArgParser = commandLineArgParser;
            this.settingProvider = settingProvider;
            PageSize = 50; //NOTE: Do we need to change this?

            SearchCommand = this.CreateCommand(Search, vm => vm.CanSearch);
            CancelSearchCommand = this.CreateCommand(CancelSearch, vm => vm.CanCancelSearch);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            RestoreRecentSearchEntries();

            if (!string.IsNullOrEmpty(commandLineArgParser.ParsedOptions.SearchQuery))
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

        public ICommand SearchCommand { get; private set; }

        public ICommand CancelSearchCommand { get; private set; }

        public void Search(string searchQuery, bool performSearch = true)
        {
            SearchQuery = searchQuery;
            SearchInProgress = !SearchQuery.IsEmpty();
            SearchEnabled = !SearchQuery.IsEmpty();
            NotifyPropertiesChanged();

            if (performSearch) Search();
        }

        public void Search()
        {
            SearchInProgress = true;
            AddRecentSearchEntry(SearchQuery);
            Parent.RefreshMessages(SelectedEndpoint, 1, SearchQuery);
        }

        public void CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
            Parent.RefreshMessages(SelectedEndpoint, 1, SearchQuery);
        }

        public void SetupPaging(PagedResult<StoredMessage> pagedResult)
        {
            Result = pagedResult.Result;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;

            NotifyPropertiesChanged();
        }

        public void RefreshResult()
        {
            Parent.RefreshMessages(SelectedEndpoint, CurrentPage, SearchQuery);
        }

        public bool CanGoToLastPage
        {
            get { return CurrentPage < PageCount && !WorkInProgress; }
        }

        public bool CanCancelSearch
        {
            get { return SearchInProgress; }
        }

        public new MessageListViewModel Parent
        {
            get { return base.Parent as MessageListViewModel; }
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
            get { return CurrentPage > 1 && !WorkInProgress; }
        }

        public bool CanGoToPreviousPage
        {
            get { return CurrentPage - 1 >= 1 && !WorkInProgress; }
        }

        public bool CanGoToNextPage
        {
            get { return CurrentPage + 1 <= PageCount && !WorkInProgress; }
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
            get { return !WorkInProgress && !string.IsNullOrWhiteSpace(SearchQuery); }
        }

        public bool CanRefreshResult
        {
            get { return !WorkInProgress; }
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
                SearchEnabled = true;
            }
        }

        public virtual void Handle(SelectedExplorerItemChanged @event)
        {
            var endpointNode = @event.SelectedExplorerItem as EndpointExplorerItem;
            if (endpointNode != null)
            {
                SelectedEndpoint = endpointNode.Endpoint;
            }

            var serviceNode = @event.SelectedExplorerItem as ServiceControlExplorerItem;
            if (serviceNode != null)
            {
                SelectedEndpoint = null;
                SearchEnabled = true;
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

            var setting = settingProvider.GetSettings<ProfilerSettings>();
            if (!setting.RecentSearchEntries.Contains(searchQuery, StringComparer.OrdinalIgnoreCase))
            {
                RecentSearchQueries.Insert(0, searchQuery);
                setting.RecentSearchEntries.Insert(0, searchQuery);

                while (RecentSearchQueries.Count > MAX_SAVED_SEARCHES)
                    RecentSearchQueries.RemoveAt(RecentSearchQueries.Count - 1);

                while (setting.RecentSearchEntries.Count > MAX_SAVED_SEARCHES)
                    setting.RecentSearchEntries.RemoveAt(setting.RecentSearchEntries.Count - 1);

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