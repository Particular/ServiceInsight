namespace Particular.ServiceInsight.Desktop.MessageList
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Caliburn.PresentationFramework;
    using Caliburn.PresentationFramework.ApplicationModel;
    using Caliburn.PresentationFramework.Screens;
    using Core.UI;
    using DevExpress.Xpf.Grid;
    using Events;
    using ExceptionHandler;
    using Explorer;
    using Explorer.EndpointExplorer;
    using ExtensionMethods;
    using MessageProperties;
    using Models;
    using Search;
    using ServiceControl;
    using Shell;
    using Shell.Menu;

    public class MessageListViewModel : Conductor<IScreen>.Collection.AllActive, IMessageListViewModel
    {
        IEventAggregator eventAggregator;
        DefaultServiceControl serviceControl;
        IGeneralHeaderViewModel generalHeaderDisplay;
        IClipboard clipboard;
        IMenuItem retryMessageMenu;
        IMenuItem copyMessageIdMenu;
        IMenuItem copyHeadersMenu;
        bool lockUpdate;
        string lastSortColumn;
        bool lastSortOrderAscending;
        int workCount;
        IMessageListView view;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            DefaultServiceControl serviceControl,
            ISearchBarViewModel searchBarViewModel,
            IGeneralHeaderViewModel generalHeaderDisplay,
            IClipboard clipboard)
        {
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
            this.generalHeaderDisplay = generalHeaderDisplay;
            this.clipboard = clipboard;

            SearchBar = searchBarViewModel;
            Items.Add(SearchBar);

            retryMessageMenu = new MenuItem("Retry Message", new RelayCommand(RetryMessage, CanRetryMessage), Properties.Resources.MessageReturn);
            copyMessageIdMenu = new MenuItem("Copy Message URI", new RelayCommand(CopyMessageId, CanCopyMessageId));
            copyHeadersMenu = new MenuItem("Copy Headers", new RelayCommand(CopyHeaders, CanCopyHeaders));

            Rows = new BindableCollection<StoredMessage>();
            ContextMenuItems = new BindableCollection<IMenuItem>
            {
                retryMessageMenu, 
                copyHeadersMenu, 
                copyMessageIdMenu
            };
        }

        public IObservableCollection<IMenuItem> ContextMenuItems { get; private set; }

        public void OnContextMenuOpening()
        {
            retryMessageMenu.IsVisible = CanRetryMessage();
            copyMessageIdMenu.IsEnabled = CanCopyMessageId();
            copyHeadersMenu.IsEnabled = CanCopyHeaders();
            NotifyPropertiesChanged();
        }

        public new IShellViewModel Parent { get { return (IShellViewModel)base.Parent; } }

        public ISearchBarViewModel SearchBar { get; private set; }

        public IObservableCollection<StoredMessage> Rows { get; private set; }

        public StoredMessage FocusedRow { get; set; }

        public bool WorkInProgress { get { return workCount > 0 && !Parent.AutoRefresh; } }

        public bool ShouldLoadMessageBody { get; set; }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public async void RetryMessage()
        {
            eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", FocusedRow.SendingEndpoint));
            var msg = FocusedRow;
            await serviceControl.RetryMessage(FocusedRow.Id);
            Rows.Remove(msg);
            eventAggregator.Publish(new WorkFinished());
        }

        public void CopyMessageId()
        {
            clipboard.CopyTo(serviceControl.GetUri(FocusedRow).ToString());
        }

        public void CopyHeaders()
        {
            clipboard.CopyTo(generalHeaderDisplay.HeaderContent);
        }

        public bool CanRetryMessage()
        {
            return FocusedRow != null &&
                   (FocusedRow.Status == MessageStatus.Failed || FocusedRow.Status == MessageStatus.RepeatedFailure)
                   && FocusedRow.Status != MessageStatus.ArchivedFailure;
        }

        public bool CanCopyHeaders()
        {
            return !generalHeaderDisplay.HeaderContent.IsEmpty();
        }

        public bool CanCopyMessageId()
        {
            return FocusedRow != null;
        }

        public override void AttachView(object view, object context)
        {
            this.view = view as IMessageListView;
            base.AttachView(view, context);
        }

        public void Focus(StoredMessage msg)
        {
            //TODO: ViewModel should have no knowledge of View or the elements in it.

            var grid = ((GridControl)((FrameworkElement)view).FindName("grid"));

            for (var i = 0; i < Rows.Count; i++)
            {
                var row = Rows[i];
                if (row.MessageId == msg.MessageId && row.TimeSent == msg.TimeSent && row.Id == msg.Id)
                {
                    grid.UnselectAll();
                    FocusedRow = row;
                    return;
                }
            }
        }

        public async void OnFocusedRowChanged()
        {
            if (lockUpdate) return;

            await LoadMessageBody();

            eventAggregator.Publish(new SelectedMessageChanged(FocusedRow));

            NotifyPropertiesChanged();
        }

        public async Task RefreshMessages(string orderBy = null, bool ascending = false)
        {
            var serviceControl = SelectedExplorerItem.As<ServiceControlExplorerItem>();
            if (serviceControl != null)
            {
                await RefreshMessages(searchQuery: SearchBar.SearchQuery,
                                      endpoint: null,
                                      orderBy: orderBy,
                                      ascending: ascending);
            }

            var endpointNode = SelectedExplorerItem.As<AuditEndpointExplorerItem>();
            if (endpointNode != null)
            {
                await RefreshMessages(searchQuery: SearchBar.SearchQuery,
                                      endpoint: endpointNode.Endpoint,
                                      orderBy: orderBy,
                                      ascending: ascending);
            }
        }

        public async Task RefreshMessages(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false)
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
                pagedResult = await serviceControl.GetAuditMessages(endpoint,
                                                                     pageIndex: pageIndex,
                                                                     searchQuery: searchQuery,
                                                                     orderBy: lastSortColumn,
                                                                     ascending: lastSortOrderAscending);
            }
            else if (!searchQuery.IsEmpty())
            {
                pagedResult = await serviceControl.Search(pageIndex: pageIndex,
                                                           searchQuery: searchQuery,
                                                           orderBy: lastSortColumn,
                                                           ascending: lastSortOrderAscending);
            }
            else
            {
                pagedResult = await serviceControl.Search(pageIndex: pageIndex,
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

        public async void Handle(BodyTabSelectionChanged @event)
        {
            ShouldLoadMessageBody = @event.IsSelected;
            if (ShouldLoadMessageBody)
            {
                var bodyLoaded = await LoadMessageBody();
                if(bodyLoaded) eventAggregator.Publish(new SelectedMessageChanged(FocusedRow));
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

        public async void OnSelectedExplorerItemChanged()
        {
            await RefreshMessages();
            NotifyPropertiesChanged();
        }

        void TryRebindMessageList(PagedResult<StoredMessage> pagedResult)
        {
            try
            {
                lockUpdate = !ShouldUpdateMessages(pagedResult);

                using (new GridFocusedRowPreserver<StoredMessage>(this))
                {
                    Rows.Clear();
                    Rows.AddRange(pagedResult.Result);
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

        async Task<bool> LoadMessageBody()
        {
            if (FocusedRow == null || !ShouldLoadMessageBody || FocusedRow.BodyUrl.IsEmpty()) return false;

            eventAggregator.Publish(new WorkStarted("Loading message body..."));

            var body = await serviceControl.GetBody(FocusedRow.BodyUrl);

            FocusedRow.Body = body;

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