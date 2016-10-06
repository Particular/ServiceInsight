namespace ServiceInsight.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Input;

    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Models;
    using Pirac;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Rx;
    using ServiceInsight.Framework.Settings;
    using Settings;
    using Shell;
    using Startup;

    public class SearchBarViewModel : RxScreen, IWorkTracker
    {
        const int MAX_SAVED_SEARCHES = 10;
        CommandLineArgParser commandLineArgParser;
        ISettingsProvider settingProvider;
        int workCount;
        IRxEventAggregator eventAggregator;

        public SearchBarViewModel(CommandLineArgParser commandLineArgParser, ISettingsProvider settingProvider, IRxEventAggregator eventAggregator)
        {
            this.commandLineArgParser = commandLineArgParser;
            this.settingProvider = settingProvider;
            PageSize = 50; //NOTE: Do we need to change this?
            this.eventAggregator = eventAggregator;

            SearchCommand = Command.Create(Search, () => CanSearch);
            CancelSearchCommand = this.WhenPropertiesChanged(nameof(SearchInProgress))
                //.Select(pcd => pcd.After)
                .Select(_ => SearchInProgress)
                .ToCommand(_ => CancelSearch());
            RefreshResultCommand = Command.Create(() => eventAggregator.Publish(new RefreshEndpointMessages(SelectedEndpoint, CurrentPage, SearchQuery)));

            GoToFirstPageCommand = CreateNavigationCommand(nameof(CanGoToFirstPage), _ => CanGoToFirstPage, 1);
            GoToPreviousPageCommand = CreateNavigationCommand(nameof(CanGoToPreviousPage), _ => CanGoToPreviousPage, CurrentPage - 1);
            GoToNextPageCommand = CreateNavigationCommand(nameof(CanGoToNextPage), _ => CanGoToNextPage, CurrentPage + 1);
            GoToLastPageCommand = CreateNavigationCommand(nameof(CanGoToLastPage), _ => CanGoToLastPage, PageCount);

            eventAggregator.GetEvent<SelectedExplorerItemChanged>().Subscribe(Handle);
            eventAggregator.GetEvent<WorkStarted>().Subscribe(Handle);
            eventAggregator.GetEvent<WorkFinished>().Subscribe(Handle);
        }

        private ICommand CreateNavigationCommand(string canExecuteName, Func<PropertyChangedData, bool> selector, int pageNum)
        {
            return this.WhenPropertiesChanged(canExecuteName)
                //.Select(pcd => pcd.After)
                .Select(selector)
                .ToCommand(_ => eventAggregator.Publish(new RefreshEndpointMessages(SelectedEndpoint, pageNum, SearchQuery)));
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

        public ICommand SearchCommand { get; }

        public ICommand CancelSearchCommand { get; }

        public ICommand RefreshResultCommand { get; }

        public ICommand GoToFirstPageCommand { get; }

        public ICommand GoToPreviousPageCommand { get; }

        public ICommand GoToNextPageCommand { get; }

        public ICommand GoToLastPageCommand { get; }

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
            eventAggregator.Publish(new RefreshEndpointMessages(SelectedEndpoint, 1, SearchQuery));
        }

        public void CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
            eventAggregator.Publish(new RefreshEndpointMessages(SelectedEndpoint, 1, SearchQuery));
        }

        public void SetupPaging(PagedResult<StoredMessage> pagedResult)
        {
            Result = pagedResult.Result;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;

            NotifyPropertiesChanged();
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

        public bool WorkInProgress => workCount > 0;

        public Endpoint SelectedEndpoint { get; private set; }

        public string SearchQuery { get; set; }

        public string SearchResultMessage => GetSearchResultMessage();

        public string SearchResultHeader => GetSearchResultHeader();

        public string SearchResultResults => GetSearchResultResults();

        public bool IsVisible { get; set; }

        public bool CanGoToFirstPage => CurrentPage > 1 && !WorkInProgress;

        public bool CanGoToPreviousPage => CurrentPage - 1 >= 1 && !WorkInProgress;

        public bool CanGoToNextPage => CurrentPage + 1 <= PageCount && !WorkInProgress;

        public bool CanGoToLastPage => CurrentPage < PageCount && !WorkInProgress;

        public IList<StoredMessage> Result { get; private set; }

        public IList<string> RecentSearchQueries { get; private set; }

        public int CurrentPage { get; private set; }

        public int PageSize { get; }

        public int TotalItemCount { get; private set; }

        public bool SearchInProgress { get; private set; }

        public bool SearchEnabled { get; private set; }

        public bool CanSearch => !WorkInProgress && !string.IsNullOrWhiteSpace(SearchQuery);

        public bool CanRefreshResult => !WorkInProgress;

        public void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(nameof(PageCount));
            NotifyOfPropertyChange(nameof(CanGoToFirstPage));
            NotifyOfPropertyChange(nameof(CanGoToLastPage));
            NotifyOfPropertyChange(nameof(CanGoToNextPage));
            NotifyOfPropertyChange(nameof(CanGoToPreviousPage));
            NotifyOfPropertyChange(nameof(CanRefreshResult));
            NotifyOfPropertyChange(nameof(SearchEnabled));
            NotifyOfPropertyChange(nameof(SearchInProgress));
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

        public void Handle(SelectedExplorerItemChanged @event)
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

        void Handle(WorkStarted @event)
        {
            workCount++;
            NotifyPropertiesChanged();
        }

        void Handle(WorkFinished @event)
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
            RecentSearchQueries = new List<string>(setting.RecentSearchEntries);
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