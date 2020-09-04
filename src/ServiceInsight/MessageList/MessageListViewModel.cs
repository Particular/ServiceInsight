namespace ServiceInsight.MessageList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Comparers;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Rx;
    using MessageProperties;
    using Models;
    using Search;
    using ServiceControl;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.Framework.Settings;
    using Shell;

    public class MessageListViewModel : RxConductor<RxScreen>.Collection.AllActive,
        IWorkTracker,
        IHandleWithTask<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>,
        IHandle<RetryMessage>,
        IHandle<SelectedMessageChanged>,
        IHandle<ServiceControlConnectionChanged>,
        IPersistPartLayout
    {
        readonly IClipboard clipboard;
        ISettingsProvider settingsProvider;
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        GeneralHeaderViewModel generalHeaderDisplay;
        string lastSortColumn;
        bool lastSortOrderAscending;
        IMessageListView view;
        ExplorerItem selectedExplorerItem;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            IServiceControl serviceControl,
            SearchBarViewModel searchBarViewModel,
            GeneralHeaderViewModel generalHeaderDisplay,
            MessageSelectionContext selectionContext,
            IClipboard clipboard,
            ISettingsProvider settingsProvider)
        {
            SearchBar = searchBarViewModel;
            Selection = selectionContext;

            this.clipboard = clipboard;
            this.settingsProvider = settingsProvider;
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.serviceControl = serviceControl;
            this.generalHeaderDisplay = generalHeaderDisplay;

            Items.Add(SearchBar);

            RetryMessageCommand = new RetryMessageCommand(eventAggregator, workNotifier, serviceControl);
            CopyMessageIdCommand = new CopyMessageURICommand(clipboard, serviceControl);
            CopyHeadersCommand = Command.Create(CopyHeaders,
                generalHeaderDisplay.Changed
                .Where(pc => pc.PropertyName == nameof(GeneralHeaderViewModel.HeaderContent))
                .Select(s => !((string)s.Value).IsEmpty()));
            Rows = new BindableCollection<StoredMessage>();
        }

        public new ShellViewModel Parent => (ShellViewModel)base.Parent;

        public SearchBarViewModel SearchBar { get; }

        public IObservableCollection<StoredMessage> Rows { get; }

        public MessageSelectionContext Selection { get; }

        public int WorkCount { get; private set; }

        public bool WorkInProgress => WorkCount > 0 && !Parent.AutoRefresh;

        public ICommand RetryMessageCommand { get; }

        public ICommand CopyMessageIdCommand { get; }

        public ICommand CopyHeadersCommand { get; }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = (IMessageListView)view;

            RestoreLayout();
        }

        public void CopyHeaders()
        {
            clipboard.CopyTo(generalHeaderDisplay.HeaderContent);
        }

        public async Task RefreshMessages(string link)
        {
            using (workNotifier.NotifyOfWork("Loading messages..."))
            {
                var pagedResult = await serviceControl.GetAuditMessages(link);

                if (pagedResult == null)
                {
                    return;
                }

                TryRebindMessageList(pagedResult);

                SearchBar.IsVisible = true;
                SearchBar.SetupPaging(pagedResult);
            }
        }

        public async Task RefreshMessages(string orderBy, bool ascending)
        {
            lastSortColumn = orderBy;
            lastSortOrderAscending = ascending;
            await RefreshMessages();
        }

        public async Task RefreshMessages()
        {
            if (selectedExplorerItem is ServiceControlExplorerItem)
            {
                await RefreshMessages(null, SearchBar.SearchQuery);
            }

            if (selectedExplorerItem is AuditEndpointExplorerItem endpointNode)
            {
                await RefreshMessages(endpointNode.Endpoint, SearchBar.SearchQuery);
            }
        }

        public async Task RefreshMessages(Endpoint endpoint, string searchQuery)
        {
            using (workNotifier.NotifyOfWork($"Loading {(endpoint == null ? "all" : endpoint.Address)} messages..."))
            {
                var pagedResult = await serviceControl.GetAuditMessages(endpoint, searchQuery, lastSortColumn, lastSortOrderAscending);
                if (pagedResult == null)
                {
                    return;
                }

                TryRebindMessageList(pagedResult);

                SearchBar.IsVisible = true;
                SearchBar.SetupPaging(pagedResult);
            }
        }

        public MessageStatusIconInfo GetMessageErrorInfo(StoredMessage msg) => new MessageStatusIconInfo(msg.Status);

        public void Handle(WorkStarted @event)
        {
            WorkCount++;
        }

        public void Handle(WorkFinished @event)
        {
            if (WorkCount > 0)
            {
                WorkCount--;
            }
        }

        public async Task Handle(SelectedExplorerItemChanged @event)
        {
            selectedExplorerItem = @event.SelectedExplorerItem;
            await RefreshMessages();
            SearchBar.NotifyPropertiesChanged();
        }

        public void Handle(AsyncOperationFailed message)
        {
            WorkCount = 0;
        }

        public void Handle(RetryMessage message)
        {
            var msg = Rows.FirstOrDefault(x => x.Id == message.Id);
            if (msg != null)
            {
                msg.Status = MessageStatus.RetryIssued;
            }
        }

        public void Handle(SelectedMessageChanged message)
        {
            var msg = Selection.SelectedMessage;
            if (msg == null)
            {
                return;
            }

            var newFocusedRow = Rows.FirstOrDefault(row => row.MessageId == msg.MessageId &&
                                                           row.TimeSent == msg.TimeSent &&
                                                           row.Id == msg.Id);

            if (newFocusedRow != null)
            {
                Selection.SelectedMessage = newFocusedRow;
                SearchBar.NotifyPropertiesChanged();
            }
        }

        public void Handle(ServiceControlConnectionChanged message)
        {
            ClearResult();
        }

        void TryRebindMessageList(PagedResult<StoredMessage> pagedResult)
        {
            if (ShouldUpdateMessages(pagedResult))
            {
                BindResult(pagedResult);
            }

            if (Selection.SelectedMessage == null && Rows.Count > 0)
            {
                Selection.SelectedMessage = Rows[0];
            }
        }

        void ClearResult()
        {
            BindResult(new PagedResult<StoredMessage>());
            SearchBar.ClearPaging();
        }

        void BindResult(PagedResult<StoredMessage> pagedResult)
        {
            try
            {
                BeginDataUpdate();
                Rows.Clear();
                Rows.AddRange(pagedResult.Result);
            }
            finally
            {
                EndDataUpdate();
            }
        }

        bool ShouldUpdateMessages(PagedResult<StoredMessage> pagedResult)
        {
            var comparer = ComparerExtensions.ThenBy(Compare<Tuple<string, MessageStatus>>.OrderBy(t => t.Item1), t => t.Item2);

            Func<StoredMessage, Tuple<string, MessageStatus>> selector = m => Tuple.Create(m.Id, m.Status);

            return Rows.Select(selector).FullExcept(pagedResult.Result.Select(selector), comparer).Any();
        }

        void EndDataUpdate()
        {
            view?.EndDataUpdate();
        }

        void BeginDataUpdate()
        {
            view?.BeginDataUpdate();
        }

        public void BringIntoView(StoredMessage msg)
        {
            eventAggregator.PublishOnUIThread(new ScrollDiagramItemIntoView(msg));
        }

        public void OnSavePartLayout()
        {
            view.OnSaveLayout(settingsProvider);
        }

        void RestoreLayout()
        {
            view.OnRestoreLayout(settingsProvider);
        }
    }
}