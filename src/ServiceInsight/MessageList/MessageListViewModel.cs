namespace ServiceInsight.MessageList
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
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Framework.Events;
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
            MessageSelectionContext selectionContext,
            IClipboard clipboard)
        {
            this.SearchBar = searchBarViewModel;
            this.Selection = selectionContext;

            this.clipboard = clipboard;
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
            this.generalHeaderDisplay = generalHeaderDisplay;
            
            Items.Add(SearchBar);

            RetryMessageCommand = new RetryMessageCommand(eventAggregator, serviceControl);
            CopyMessageIdCommand = new CopyMessageURICommand(clipboard, serviceControl);
            CopyHeadersCommand = this.CreateCommand(CopyHeaders, generalHeaderDisplay.WhenAnyValue(ghd => ghd.HeaderContent).Select(s => !s.IsEmpty()));
            Rows = new BindableCollection<StoredMessage>();
        }

        public new ShellViewModel Parent { get { return (ShellViewModel)base.Parent; } }

        public SearchBarViewModel SearchBar { get; }

        public IObservableCollection<StoredMessage> Rows { get; }

        public MessageSelectionContext Selection { get; }

        public bool WorkInProgress { get { return workCount > 0 && !Parent.AutoRefresh; } }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public ICommand RetryMessageCommand { get; }

        public ICommand CopyMessageIdCommand { get; }

        public ICommand CopyHeadersCommand { get; }

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
            var msg = Selection.SelectedMessage;
            if (msg == null) return;

            var newFocusedRow = Rows.FirstOrDefault(row => row.MessageId == msg.MessageId &&
                                                           row.TimeSent == msg.TimeSent &&
                                                           row.Id == msg.Id);

            if (newFocusedRow != null)
            {
                Selection.SelectedMessage = newFocusedRow;
                NotifyPropertiesChanged();
            }
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

            if (Selection.SelectedMessage == null && Rows.Count > 0)
            {
                Selection.SelectedMessage = Rows[0];
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

        void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => SelectedExplorerItem);
            SearchBar.NotifyPropertiesChanged();
        }

        void EndDataUpdate()
        {
            view?.EndDataUpdate();
        }

        void BeginDataUpdate()
        {
            view?.BeginDataUpdate();
        }

        public void RaiseSelectedMessageChanged(StoredMessage currentItem)
        {
            Selection.SelectedMessage = currentItem;
        }

        public void BringIntoView(StoredMessage msg)
        {
            eventAggregator.Publish(new ScrollDiagramItemIntoView(msg));
        }
    }
}