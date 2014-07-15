namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Events;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using Framework;
    using Framework.Rx;
    using MessageProperties;
    using Models;
    using ReactiveUI;
    using Search;
    using ServiceControl;
    using Shell;
    using IScreen = Caliburn.Micro.IScreen;

    public class MessageListViewModel : RxConductor<IScreen>.Collection.AllActive,
        ITableViewModel<StoredMessage>,
        IWorkTracker,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<WorkStarted>,
        IHandle<WorkFinished>,
        IHandle<AsyncOperationFailed>,
        IHandle<RetryMessage>,
        IHandle<BodyTabSelectionChanged>
    {
        readonly IClipboard clipboard;
        IEventAggregator eventAggregator;
        IServiceControl serviceControl;
        GeneralHeaderViewModel generalHeaderDisplay;
        bool lockUpdate;
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

            RetryMessageCommand = this.CreateCommand(RetryMessage, vm => vm.CanRetryMessage);
            CopyMessageIdCommand = this.CreateCommand(CopyMessageId, vm => vm.CanCopyMessageId);
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

        public void RetryMessage()
        {
            eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", FocusedRow.SendingEndpoint));
            var msg = FocusedRow;
            serviceControl.RetryMessage(FocusedRow.Id);
            Rows.Remove(msg);
            eventAggregator.Publish(new WorkFinished());
        }

        public void CopyMessageId()
        {
            clipboard.CopyTo(serviceControl.CreateServiceInsightUri(FocusedRow).ToString());
        }

        public void CopyHeaders()
        {
            clipboard.CopyTo(generalHeaderDisplay.HeaderContent);
        }

        public bool CanRetryMessage
        {
            get
            {
                return FocusedRow != null &&
                       (FocusedRow.Status == MessageStatus.Failed || FocusedRow.Status == MessageStatus.RepeatedFailure)
                       && FocusedRow.Status != MessageStatus.ArchivedFailure;
            }
        }

        public bool CanCopyMessageId
        {
            get { return FocusedRow != null; }
        }

        public void Focus(StoredMessage msg)
        {
            FocusedRow = Rows.FirstOrDefault(row => row.MessageId == msg.MessageId && row.TimeSent == msg.TimeSent && row.Id == msg.Id);
        }

        void DoFocusedRowChanged()
        {
            if (lockUpdate) return;

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
            var msg = Rows.FirstOrDefault(x => x.MessageId == message.MessageId);
            if (msg != null)
            {
                msg.Status = MessageStatus.RetryIssued;
            }
        }

        public void OnSelectedExplorerItemChanged()
        {
            RefreshMessages();
            NotifyPropertiesChanged();
        }

        void TryRebindMessageList(PagedResult<StoredMessage> pagedResult)
        {
            try
            {
                lockUpdate = !ShouldUpdateMessages(pagedResult);

                if (!lockUpdate)
                {
                    using (new GridFocusedRowPreserver<StoredMessage>(this))
                    {
                        Rows.Clear();
                        Rows.AddRange(pagedResult.Result);
                    }
                }
            }
            finally
            {
                lockUpdate = false;
            }

            AutoFocusFirstRow();
        }

        void AutoFocusFirstRow()
        {
            if (FocusedRow == null && Rows.Count > 0)
            {
                FocusedRow = Rows[0];
            }
        }

        bool ShouldUpdateMessages(PagedResult<StoredMessage> pagedResult)
        {
            if (FocusedRow == null)
                return true;

            var hasNewMessageInConversation = Rows.Count(m => m.ConversationId == FocusedRow.ConversationId) != pagedResult.Result.Count(p => p.ConversationId == FocusedRow.ConversationId);
            if (hasNewMessageInConversation)
                return true;

            var messagesInConversation = Rows.Where(m => m.ConversationId == FocusedRow.ConversationId);
            var anyConversationMessageChanged = messagesInConversation.Any(message => ShouldUpdateMessage(message, pagedResult.Result.FirstOrDefault(m => m.Id == message.Id)));

            return anyConversationMessageChanged;
        }

        static bool ShouldUpdateMessage(StoredMessage focusedMessage, StoredMessage newMessage)
        {
            return newMessage == null || newMessage.DisplayPropertiesChanged(focusedMessage);
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