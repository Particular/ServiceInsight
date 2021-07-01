namespace ServiceInsight.MessageList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Commands;
    using Framework.Events;
    using Framework.Rx;
    using Framework.Settings;
    using MessageProperties;
    using Models;
    using Nito.Comparers;
    using Search;
    using ServiceControl;
    using Shell;

    public class MessageListViewModel : RxConductor<RxScreen>.Collection.AllActive,
        IWorkTracker,
        IHandleWithTask<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>,
        IHandle<RetryMessage>,
        IHandle<SelectedMessageChanged>,
        IHandle<ServiceControlDisconnected>,
        IPersistPartLayout
    {
        readonly IClipboard clipboard;
        readonly ISettingsProvider settingsProvider;
        readonly ServiceControlClientRegistry clientRegistry;
        readonly IEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly GeneralHeaderViewModel generalHeaderDisplay;

        string lastSortColumn;
        bool lastSortOrderAscending;
        IMessageListView view;
        ExplorerItem selectedExplorerItem;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            SearchBarViewModel searchBarViewModel,
            GeneralHeaderViewModel generalHeaderDisplay,
            MessageSelectionContext selectionContext,
            IClipboard clipboard,
            ISettingsProvider settingsProvider,
            ServiceControlClientRegistry clientRegistry)
        {
            SearchBar = searchBarViewModel;
            Selection = selectionContext;

            this.clipboard = clipboard;
            this.settingsProvider = settingsProvider;
            this.clientRegistry = clientRegistry;
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.generalHeaderDisplay = generalHeaderDisplay;

            Items.Add(SearchBar);

            RetryMessageCommand = new RetryMessageCommand(eventAggregator, workNotifier, this);
            CopyMessageIdCommand = new CopyMessageURICommand(clipboard, this);
            CopyHeadersCommand = Command.Create(
                CopyHeaders,
                generalHeaderDisplay.Changed
                    .Where(pc => pc.PropertyName == nameof(GeneralHeaderViewModel.HeaderContent))
                    .Select(s => !((string)s.Value).IsEmpty()));
            Rows = new BindableCollection<StoredMessage>();
        }

        public new ShellViewModel Parent => (ShellViewModel)base.Parent;

        public SearchBarViewModel SearchBar { get; }

        public IObservableCollection<StoredMessage> Rows { get; }

        public IServiceControl ServiceControl { get; private set; }

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
                var pagedResult = default(PagedResult<StoredMessage>);

                if (ServiceControl != null)
                {
                    pagedResult = await ServiceControl.GetAuditMessages(link);
                }

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
                await RefreshMessages(null, SearchBar.SearchQuery, pageNo: SearchBar.CurrentPage);
            }

            if (selectedExplorerItem is AuditEndpointExplorerItem endpointNode)
            {
                await RefreshMessages(endpointNode.Endpoint, SearchBar.SearchQuery, pageNo: SearchBar.CurrentPage);
            }
        }

        public async Task RefreshMessages(Endpoint endpoint, string searchQuery, int? pageNo = null)
        {
            var pagedResult = default(PagedResult<StoredMessage>);

            using (workNotifier.NotifyOfWork($"Loading {(endpoint == null ? "all" : endpoint.Address)} messages..."))
            {
                if (ServiceControl != null)
                {
                    pagedResult = await ServiceControl.GetAuditMessages(endpoint, pageNo: pageNo, searchQuery, lastSortColumn, lastSortOrderAscending);
                }

                if (pagedResult?.Result == null || pagedResult.Result.Count == 0)
                {
                    ClearResult();
                    return;
                }

                TryRebindMessageList(pagedResult);

                SearchBar.IsVisible = true;
                SearchBar.SetupPaging(pagedResult);
            }
        }

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
            ServiceControl = @event.SelectedExplorerItem.GetServiceControlClient(clientRegistry);

            if (ServiceControl != null)
            {
                await RefreshMessages();
                SearchBar.NotifyPropertiesChanged();
            }
            else
            {
                ClearResult();
            }
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
                msg.NotifyOfPropertyChange(nameof(msg.Status));
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
            var messageStatusOrder = ComparerBuilder.For<Tuple<string, MessageStatus>>().OrderBy(t => t.Item1);
            var comparer = messageStatusOrder.ThenBy(t => t.Item2);

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

        public void Handle(ServiceControlDisconnected message)
        {
            if (selectedExplorerItem == null || selectedExplorerItem == message.ExplorerItem)
            {
                selectedExplorerItem = null;
                ClearResult();
            }
        }
    }
}