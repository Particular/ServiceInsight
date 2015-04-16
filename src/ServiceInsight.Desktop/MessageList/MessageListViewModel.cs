namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Rx;
    using MessageProperties;
    using Models;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using ReactiveUI;
    using Search;
    using ServiceControl;
    using Shell;

    public class MessageListViewModel : RxConductor<RxScreen>.Collection.AllActive,
        IWorkTracker,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>,
        IHandle<RetryMessage>,
        IHandle<BodyTabSelectionChanged>,
        IHandle<SelectedMessageChanged>
    {
        readonly IClipboard clipboard;
        IEventAggregator eventAggregator;
        IServiceControl serviceControl;
        GeneralHeaderViewModel generalHeaderDisplay;
        string lastSortColumn;
        bool lastSortOrderAscending;
        int workCount;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IServiceControl serviceControl,
            SearchBarViewModel searchBarViewModel,
            GeneralHeaderViewModel generalHeaderDisplay,
            IClipboard clipboard)
        {
            this.clipboard = clipboard;
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
            this.generalHeaderDisplay = generalHeaderDisplay;

            SearchBar = searchBarViewModel;
            Items.Add(SearchBar);

            RetryMessageCommand = new RetryMessageCommand(eventAggregator, serviceControl);
            CopyMessageIdCommand = new CopyMessageURICommand(clipboard, serviceControl);

            CopyHeadersCommand = this.CreateCommand(CopyHeaders, generalHeaderDisplay.WhenAnyValue(ghd => ghd.HeaderContent).Select(s => !s.IsEmpty()));

            Rows = new BindableCollection<StoredMessage>();

            this.WhenAnyValue(vm => vm.FocusedRow)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .Subscribe(_ => DoFocusedRowChanged());
        }

        public new ShellViewModel Parent { get { return (ShellViewModel)base.Parent; } }

        public SearchBarViewModel SearchBar { get; private set; }

        public IObservableCollection<StoredMessage> Rows { get; private set; }

        public StoredMessage FocusedRow { get; set; }

        public bool WorkInProgress { get { return workCount > 0 && !Parent.AutoRefresh; } }

        public bool ShouldLoadMessageBody { get; set; }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public ICommand RetryMessageCommand { get; private set; }

        public ICommand CopyMessageIdCommand { get; private set; }

        public ICommand CopyHeadersCommand { get; private set; }

        public void CopyHeaders()
        {
            clipboard.CopyTo(generalHeaderDisplay.HeaderContent);
        }

        public void Focus(StoredMessage msg)
        {
            if (msg == null)
            {
                FocusedRow = null;
                return;
            }

            FocusedRow = Rows.FirstOrDefault(row => row.MessageId == msg.MessageId && row.TimeSent == msg.TimeSent && row.Id == msg.Id);
        }

        void DoFocusedRowChanged()
        {
            LoadMessageBody();

            eventAggregator.Publish(new SelectedMessageChanged(FocusedRow));

            NotifyPropertiesChanged();
        }

        public void RefreshMessages(string orderBy = null, bool ascending = false)
        {
            var serviceControl = SelectedExplorerItem as ServiceControlExplorerItem;
            if (serviceControl != null)
            {
                RefreshMessages(searchQuery: SearchBar.SearchQuery,
                                     endpoint: null,
                                     orderBy: orderBy,
                                     ascending: ascending);

                // The DX Grid doesn't update properly
                // So this refreshes the focused value
                // when selecting the SC node.
                var temp = FocusedRow;
                FocusedRow = null;
                FocusedRow = temp;
            }

            var endpointNode = SelectedExplorerItem as AuditEndpointExplorerItem;
            if (endpointNode != null)
            {
                RefreshMessages(searchQuery: SearchBar.SearchQuery,
                                     endpoint: endpointNode.Endpoint,
                                     orderBy: orderBy,
                                     ascending: ascending);
            }
        }

        public void RefreshMessages(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false)
        {
            eventAggregator.Publish(new WorkStarted("Loading {0} messages...", endpoint == null ? "all" : endpoint.Address));

            if (orderBy != null)
            {
                lastSortColumn = orderBy;
                lastSortOrderAscending = ascending;
            }

            PagedResult<StoredMessage> pagedResult;

            if (endpoint != null)
            {
                pagedResult = serviceControl.GetAuditMessages(endpoint,
                                                                     pageIndex: pageIndex,
                                                                     searchQuery: searchQuery,
                                                                     orderBy: lastSortColumn,
                                                                     ascending: lastSortOrderAscending);
            }
            else if (!searchQuery.IsEmpty())
            {
                pagedResult = serviceControl.Search(pageIndex: pageIndex,
                                                           searchQuery: searchQuery,
                                                           orderBy: lastSortColumn,
                                                           ascending: lastSortOrderAscending);
            }
            else
            {
                pagedResult = serviceControl.Search(pageIndex: pageIndex,
                                                           searchQuery: null,
                                                           orderBy: lastSortColumn,
                                                           ascending: lastSortOrderAscending);
            }

            TryRebindMessageList(pagedResult);

            SearchBar.IsVisible = true;
            SearchBar.SetupPaging(new PagedResult<StoredMessage>
            {
                CurrentPage = pagedResult.CurrentPage,
                TotalCount = pagedResult.TotalCount,
                Result = pagedResult.Result,
            });

            eventAggregator.Publish(new WorkFinished());
        }

        public MessageErrorInfo GetMessageErrorInfo(StoredMessage msg)
        {
            return new MessageErrorInfo(msg.Status);
        }

        public void Handle(WorkStarted @event)
        {
            workCount++;
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public void Handle(WorkFinished @event)
        {
            if (workCount > 0)
            {
                workCount--;
                NotifyOfPropertyChange(() => WorkInProgress);
            }
        }

        public void Handle(BodyTabSelectionChanged @event)
        {
            ShouldLoadMessageBody = @event.IsSelected;
            if (ShouldLoadMessageBody)
            {
                var bodyLoaded = LoadMessageBody();
                if (bodyLoaded) eventAggregator.Publish(new SelectedMessageChanged(FocusedRow));
            }
        }

        public void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }

        public void Handle(AsyncOperationFailed message)
        {
            workCount = 0;
            NotifyOfPropertyChange(() => WorkInProgress);
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
            Focus(message.Message);
        }

        public void OnSelectedExplorerItemChanged()
        {
            RefreshMessages();
            NotifyPropertiesChanged();
        }

        void TryRebindMessageList(PagedResult<StoredMessage> pagedResult)
        {
            if (ShouldUpdateMessages(pagedResult))
            {
                var currentItem = FocusedRow;

                Rows.Clear();
                Rows.AddRange(pagedResult.Result);

                if (currentItem != null)
                    FocusedRow = Rows.FirstOrDefault(item => item.Id == currentItem.Id);
            }

            if (FocusedRow == null && Rows.Count > 0)
            {
                FocusedRow = Rows[0];
            }
        }

        bool ShouldUpdateMessages(PagedResult<StoredMessage> pagedResult)
        {
            return Rows.Select(m => m.Id).FullExcept(pagedResult.Result.Select(m => m.Id)).Any();
        }

        bool LoadMessageBody()
        {
            if (FocusedRow == null || !ShouldLoadMessageBody || FocusedRow.BodyUrl.IsEmpty()) return false;

            eventAggregator.Publish(new WorkStarted("Loading message body..."));

            serviceControl.LoadBody(FocusedRow);

            //var body = serviceControl.GetBody(FocusedRow.BodyUrl);

            //FocusedRow.Body = body;

            eventAggregator.Publish(new WorkFinished());

            return true;
        }

        void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => SelectedExplorerItem);
            SearchBar.NotifyPropertiesChanged();
        }
    }
}