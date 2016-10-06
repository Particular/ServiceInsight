namespace ServiceInsight.MessageList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Comparers;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Rx;
    using MessageProperties;
    using Models;
    using Pirac;
    using Search;
    using ServiceControl;
    using ServiceInsight.Framework.Commands;
    using ServiceInsight.Framework.Events;
    using Shell;

    public class MessageListViewModel : RxConductor<RxScreen>.RxCollection.AllActive, IWorkTracker
    {
        readonly IClipboard clipboard;
        IRxEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        GeneralHeaderViewModel generalHeaderDisplay;
        string lastSortColumn;
        bool lastSortOrderAscending;
        int workCount;
        IMessageListView view;

        public MessageListViewModel(
            IRxEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            IServiceControl serviceControl,
            SearchBarViewModel searchBarViewModel,
            GeneralHeaderViewModel generalHeaderDisplay,
            MessageSelectionContext selectionContext,
            IClipboard clipboard)
        {
            SearchBar = searchBarViewModel;
            Selection = selectionContext;

            this.clipboard = clipboard;
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.serviceControl = serviceControl;
            this.generalHeaderDisplay = generalHeaderDisplay;

            AddChildren(SearchBar);

            RetryMessageCommand = new RetryMessageCommand(eventAggregator, workNotifier, serviceControl);
            CopyMessageIdCommand = new CopyMessageURICommand(clipboard, serviceControl);
            CopyHeadersCommand = generalHeaderDisplay
                .WhenPropertiesChanged<string>(nameof(GeneralHeaderViewModel.HeaderContent))
                //.Select(pcd => !pcd.After.IsEmpty())
                .Select(_ => !generalHeaderDisplay.HeaderContent.IsEmpty())
                .ToCommand(_ => CopyHeaders());
            Rows = new List<StoredMessage>();

            eventAggregator.GetEvent<SelectedExplorerItemChanged>().Subscribe(Handle);
            eventAggregator.GetEvent<WorkStarted>().Subscribe(Handle);
            eventAggregator.GetEvent<WorkFinished>().Subscribe(Handle);
            eventAggregator.GetEvent<AsyncOperationFailed>().Subscribe(Handle);
            eventAggregator.GetEvent<RetryMessage>().Subscribe(Handle);
            eventAggregator.GetEvent<SelectedMessageChanged>().Subscribe(Handle);
            eventAggregator.GetEvent<RefreshEndpointMessages>().Subscribe(m => RefreshMessages(m.Endpoint, m.PageIndex, m.SearchQuery, m.OrderBy, m.Ascending));

            this.WhenPropertiesChanged(nameof(SelectedExplorerItem))
                .ObserveOnPiracMain()
                .Subscribe(_ =>
                {
                    RefreshMessages();
                });
        }

        public SearchBarViewModel SearchBar { get; }

        public IList<StoredMessage> Rows { get; }

        public MessageSelectionContext Selection { get; }

        public bool AutoRefresh { get; set; }

        public bool WorkInProgress => workCount > 0 && !AutoRefresh;

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

        void RefreshMessages(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false)
        {
            using (workNotifier.NotifyOfWork($"Loading {(endpoint == null ? "all" : endpoint.Address)} messages..."))
            {
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
        }

        public MessageErrorInfo GetMessageErrorInfo(StoredMessage msg) => new MessageErrorInfo(msg.Status);

        void Handle(WorkStarted @event)
        {
            Interlocked.Increment(ref workCount);
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        void Handle(WorkFinished @event)
        {
            Interlocked.Decrement(ref workCount);
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }

        void Handle(AsyncOperationFailed message)
        {
            workCount = 0;
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        void Handle(RetryMessage message)
        {
            var msg = Rows.FirstOrDefault(x => x.Id == message.Id);
            if (msg != null)
            {
                msg.Status = MessageStatus.RetryIssued;
            }
        }

        void Handle(SelectedMessageChanged message)
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
                NotifyPropertiesChanged();
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