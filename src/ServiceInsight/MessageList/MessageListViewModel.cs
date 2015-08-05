namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
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
        IMessageListView view;

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

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            this.view = (IMessageListView)view;
        }

        public void CopyHeaders()
        {
            clipboard.CopyTo(generalHeaderDisplay.HeaderContent);
        }

        public void RefreshMessages(string orderBy = null, bool ascending = false)
        {
            var serviceControlExplorerItem = SelectedExplorerItem as ServiceControlExplorerItem;
            if (serviceControlExplorerItem != null)
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
            try
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

                if (pagedResult == null)
                {
                    return;
                }

                TryRebindMessageList(pagedResult);

                SearchBar.IsVisible = true;
                SearchBar.SetupPaging(new PagedResult<StoredMessage>
                {
                    CurrentPage = pagedResult.CurrentPage,
                    TotalCount = pagedResult.TotalCount,
                    Result = pagedResult.Result,
                });
            }
            finally
            {
                eventAggregator.Publish(new WorkFinished());
            }
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
            var msg = message.Message;
            if (msg == null)
            {
                FocusedRow = null;
                return;
            }

            var newFocusedRow = Rows.FirstOrDefault(row => row.MessageId == msg.MessageId && row.TimeSent == msg.TimeSent && row.Id == msg.Id);
            if (newFocusedRow == null)
            {
                FocusedRow = null;
                return;
            }

            FocusedRow = newFocusedRow;

            LoadMessageBody();

            NotifyPropertiesChanged();
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
                BindResult(pagedResult);
            }

            if (FocusedRow == null && Rows.Count > 0)
            {
                FocusedRow = Rows[0];
            }
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

        bool LoadMessageBody()
        {
            if (FocusedRow == null || !ShouldLoadMessageBody || FocusedRow.BodyUrl.IsEmpty())
            {
                return false;
            }

            eventAggregator.Publish(new WorkStarted("Loading message body..."));

            serviceControl.LoadBody(FocusedRow);

            eventAggregator.Publish(new WorkFinished());

            return true;
        }

        void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => SelectedExplorerItem);
            SearchBar.NotifyPropertiesChanged();
        }

        void EndDataUpdate()
        {
            if (view != null)
            {
                view.EndDataUpdate();
            }
        }

        void BeginDataUpdate()
        {
            if (view != null)
            {
                view.BeginDataUpdate();
            }
        }

        public void RaiseSelectedMessageChanged(StoredMessage currentItem)
        {
            eventAggregator.Publish(new SelectedMessageChanged(currentItem));
        }
    }
}