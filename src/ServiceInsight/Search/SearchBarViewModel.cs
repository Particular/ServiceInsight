namespace ServiceInsight.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using MessageList;
    using Models;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Rx;
    using ServiceInsight.Framework.Settings;
    using Settings;
    using Shell;
    using Startup;

    public class SearchBarViewModel : RxScreen,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IWorkTracker
    {
        const int MAX_SAVED_SEARCHES = 10;
        CommandLineArgParser commandLineArgParser;
        ISettingsProvider settingProvider;
        int workCount;

        public SearchBarViewModel(CommandLineArgParser commandLineArgParser, ISettingsProvider settingProvider)
        {
            this.commandLineArgParser = commandLineArgParser;
            this.settingProvider = settingProvider;

            SearchCommand = Command.Create(this, Search, vm => vm.CanSearch);
            CancelSearchCommand = Command.Create(this, CancelSearch, vm => vm.CanCancelSearch);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            RestoreRecentSearchEntries();

            if (!string.IsNullOrEmpty(commandLineArgParser.ParsedOptions.SearchQuery))
            {
                Search(commandLineArgParser.ParsedOptions.SearchQuery);
            }
        }

        public void GoToFirstPage()
        {
            Parent.RefreshMessages(FirstLink);
        }

        public void GoToPreviousPage()
        {
            Parent.RefreshMessages(PrevLink);
        }

        public void GoToNextPage()
        {
            Parent.RefreshMessages(NextLink);
        }

        public void GoToLastPage()
        {
            Parent.RefreshMessages(LastLink);
        }

        public ICommand SearchCommand { get; }

        public ICommand CancelSearchCommand { get; }

        public void Search(string searchQuery, bool performSearch = true)
        {
            SearchQuery = searchQuery;
            SearchInProgress = !SearchQuery.IsEmpty();
            SearchEnabled = !SearchQuery.IsEmpty();
            NotifyPropertiesChanged();

            if (performSearch)
            {
                Search();
            }
        }

        public void Search()
        {
            SearchInProgress = true;
            AddRecentSearchEntry(SearchQuery);
            Parent.RefreshMessages(SelectedEndpoint, SearchQuery);
        }

        public void CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
            Parent.RefreshMessages(SelectedEndpoint, SearchQuery);
        }

        public void SetupPaging(PagedResult<StoredMessage> pagedResult)
        {
            Result = pagedResult.Result;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;
            NextLink = pagedResult.NextLink;
            PrevLink = pagedResult.PrevLink;
            FirstLink = pagedResult.FirstLink;
            LastLink = pagedResult.LastLink;
            PageSize = pagedResult.PageSize;
            NotifyPropertiesChanged();
        }

        public void ClearPaging()
        {
            Result = new List<StoredMessage>();
            CurrentPage = 0;
            TotalItemCount = 0;
            SearchQuery = null;
            SearchInProgress = false;
            SelectedEndpoint = null;
        }

        // Required for binding convention with CanRefreshResult
        public void RefreshResult()
        {
            Search();
        }

        public bool CanGoToLastPage => LastLink != null && !WorkInProgress;

        public bool CanCancelSearch => SearchInProgress;

        public new MessageListViewModel Parent => base.Parent as MessageListViewModel;

        public bool WorkInProgress => workCount > 0;

        public Endpoint SelectedEndpoint { get; private set; }

        public string SearchQuery { get; set; }

        public string SearchResultMessage => GetSearchResultMessage();

        public string SearchResultHeader => GetSearchResultHeader();

        public string SearchResultResults => GetSearchResultResults();

        public bool IsVisible { get; set; }

        public bool CanGoToFirstPage => FirstLink != null && !WorkInProgress;

        public bool CanGoToPreviousPage => PrevLink != null && !WorkInProgress;

        public bool CanGoToNextPage => NextLink != null && !WorkInProgress;

        public IList<StoredMessage> Result { get; private set; }

        public IObservableCollection<string> RecentSearchQueries { get; private set; }

        public int CurrentPage { get; private set; }

        public int PageSize { get; private set; }

        public string NextLink { get; private set; }

        public string PrevLink { get; private set; }

        public string FirstLink { get; private set; }

        public string LastLink { get; private set; }

        public int TotalItemCount { get; private set; }

        public bool SearchInProgress { get; private set; }

        public bool SearchEnabled { get; private set; }

        public bool CanSearch => !WorkInProgress && !string.IsNullOrWhiteSpace(SearchQuery);

        public bool CanRefreshResult => !WorkInProgress;

        public void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(nameof(CanGoToFirstPage));
            NotifyOfPropertyChange(nameof(CanGoToLastPage));
            NotifyOfPropertyChange(nameof(CanGoToNextPage));
            NotifyOfPropertyChange(nameof(CanGoToPreviousPage));
            NotifyOfPropertyChange(nameof(CanRefreshResult));
            NotifyOfPropertyChange(nameof(SearchEnabled));
            NotifyOfPropertyChange(nameof(CanCancelSearch));
            NotifyOfPropertyChange(nameof(WorkInProgress));
            NotifyOfPropertyChange(nameof(SearchResultMessage));
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
            if (searchQuery.IsEmpty())
            {
                return;
            }

            var setting = settingProvider.GetSettings<ProfilerSettings>();
            if (!setting.RecentSearchEntries.Contains(searchQuery, StringComparer.OrdinalIgnoreCase))
            {
                RecentSearchQueries.Insert(0, searchQuery);
                setting.RecentSearchEntries.Insert(0, searchQuery);

                while (RecentSearchQueries.Count > MAX_SAVED_SEARCHES)
                {
                    RecentSearchQueries.RemoveAt(RecentSearchQueries.Count - 1);
                }

                while (setting.RecentSearchEntries.Count > MAX_SAVED_SEARCHES)
                {
                    setting.RecentSearchEntries.RemoveAt(setting.RecentSearchEntries.Count - 1);
                }

                settingProvider.SaveSettings(setting);
            }
        }

        string GetSearchResultMessage() => string.Format("{0}{1}", GetSearchResultHeader(), GetSearchResultResults());

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