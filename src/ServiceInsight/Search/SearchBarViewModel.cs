namespace ServiceInsight.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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
        const int MaxSavedSearches = 10;
        CommandLineArgParser commandLineArgParser;
        ISettingsProvider settingProvider;
        int workCount;
        bool isSettingUpPaging;
        bool isResttingPaging;

        public SearchBarViewModel(CommandLineArgParser commandLineArgParser, ISettingsProvider settingProvider)
        {
            this.commandLineArgParser = commandLineArgParser;
            this.settingProvider = settingProvider;

            SearchCommand = Command.CreateAsync(this, Search, vm => vm.CanSearch);
            CancelSearchCommand = Command.CreateAsync(this, CancelSearch, vm => vm.CanCancelSearch);
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            RestoreRecentSearchEntries();

            await PerformCommandLineSearch();
        }

        public async Task PerformCommandLineSearch()
        {
            if (!string.IsNullOrEmpty(commandLineArgParser.ParsedOptions.SearchQuery))
            {
                await Search(commandLineArgParser.ParsedOptions.SearchQuery);
            }
        }

        public async Task GoToFirstPage()
        {
            await Parent.RefreshMessages(FirstLink);
        }

        public async Task GoToPreviousPage()
        {
            await Parent.RefreshMessages(PrevLink);
        }

        public async Task GoToNextPage()
        {
            await Parent.RefreshMessages(NextLink);
        }

        public async Task GoToLastPage()
        {
            await Parent.RefreshMessages(LastLink);
        }

        public ICommand SearchCommand { get; }

        public ICommand CancelSearchCommand { get; }

        public async Task Search(string searchQuery, bool performSearch = true)
        {
            SearchQuery = searchQuery;
            SearchInProgress = !SearchQuery.IsEmpty();
            SearchEnabled = !SearchQuery.IsEmpty();
            NotifyPropertiesChanged();

            if (performSearch)
            {
                await Search();
            }
        }

        public async Task Search()
        {
            SearchInProgress = true;
            AddRecentSearchEntry(SearchQuery);
            await Parent.RefreshMessages(SelectedEndpoint, SearchQuery);
        }

        public async Task CancelSearch()
        {
            SearchQuery = null;
            SearchInProgress = false;
            await Parent.RefreshMessages(SelectedEndpoint, SearchQuery);
        }

        public void SetupPaging(PagedResult<StoredMessage> pagedResult)
        {
            isSettingUpPaging = true;

            Result = pagedResult.Result;
            CurrentPage = pagedResult.TotalCount > 0 ? pagedResult.CurrentPage : 0;
            TotalItemCount = pagedResult.TotalCount;
            TotalPagesCount = (pagedResult.TotalCount / pagedResult.PageSize) + (Math.DivRem(pagedResult.TotalCount, pagedResult.PageSize, out var _) == 1 ? 0 : 1);
            NextLink = pagedResult.NextLink;
            PrevLink = pagedResult.PrevLink;
            FirstLink = pagedResult.FirstLink;
            LastLink = pagedResult.LastLink;
            PageSize = pagedResult.PageSize;
            
            isSettingUpPaging = false;
            
            NotifyPropertiesChanged();
        }

        public void ClearPaging()
        {
            try
            {
                isResttingPaging = true;
                Result = new List<StoredMessage>();
                CurrentPage = 0;
                TotalItemCount = 0;
                TotalPagesCount = 0;
                SearchQuery = null;
                SearchInProgress = false;
                SelectedEndpoint = null;
                NextLink = null;
                PrevLink = null;
                LastLink = null;
                FirstLink = null;
            }
            finally
            {
                isResttingPaging = false;
            }
        }

        // Required for binding convention with CanRefreshResult
        public async Task RefreshResult()
        {
            await Search();
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

        public int CurrentPage { get; set; }

        public int PageSize { get; private set; }

        public string NextLink { get; private set; }

        public string PrevLink { get; private set; }

        public string FirstLink { get; private set; }

        public string LastLink { get; private set; }

        public int TotalItemCount { get; private set; }

        public int TotalPagesCount { get; private set; }

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

        public async void OnCurrentPageChanged()
        {
            if (!isResttingPaging && !isSettingUpPaging && CurrentPage >=0 && CurrentPage <= TotalPagesCount)
            {
                await Parent.RefreshMessages();
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

                while (RecentSearchQueries.Count > MaxSavedSearches)
                {
                    RecentSearchQueries.RemoveAt(RecentSearchQueries.Count - 1);
                }

                while (setting.RecentSearchEntries.Count > MaxSavedSearches)
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