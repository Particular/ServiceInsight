using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core.UI;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Shell.Menu;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageListViewModel : Conductor<IScreen>.Collection.AllActive, IMessageListViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceControl _serviceControl;
        private readonly IErrorHeaderViewModel _errorHeaderDisplay;
        private readonly IGeneralHeaderViewModel _generalHeaderDisplay;
        private readonly IClipboard _clipboard;
        private readonly IMenuItem _returnToSourceMenu;
        private readonly IMenuItem _retryMessageMenu;
        private readonly IMenuItem _copyMessageIdMenu;
        private readonly IMenuItem _copyHeadersMenu;
        private bool _lockUpdate;
        private string _lastSortColumn;
        private bool _lastSortOrderAscending;
        private int _workCount;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IServiceControl serviceControl,
            ISearchBarViewModel searchBarViewModel,
            IErrorHeaderViewModel errorHeaderDisplay,
            IGeneralHeaderViewModel generalHeaderDisplay,
            IClipboard clipboard)
        {
            _eventAggregator = eventAggregator;
            _serviceControl = serviceControl;
            _errorHeaderDisplay = errorHeaderDisplay;
            _generalHeaderDisplay = generalHeaderDisplay;
            _clipboard = clipboard;

            SearchBar = searchBarViewModel;
            Items.Add(SearchBar);

            _returnToSourceMenu = new MenuItem("Return To Source", new RelayCommand(ReturnToSource, CanReturnToSource), Properties.Resources.MessageReturn);
            _retryMessageMenu = new MenuItem("Retry Message", new RelayCommand(RetryMessage, CanRetryMessage), Properties.Resources.MessageReturn);
            _copyMessageIdMenu = new MenuItem("Copy Message URI", new RelayCommand(CopyMessageId, CanCopyMessageId));
            _copyHeadersMenu = new MenuItem("Copy Headers", new RelayCommand(CopyHeaders, CanCopyHeaders));

            Rows = new BindableCollection<StoredMessage>();
            SelectedRows = new BindableCollection<StoredMessage>();
            ContextMenuItems = new BindableCollection<IMenuItem>
            {
                _returnToSourceMenu, 
                _retryMessageMenu, 
                _copyHeadersMenu, 
                _copyMessageIdMenu
            };
        }

        public IObservableCollection<IMenuItem> ContextMenuItems { get; private set; }
        
        public void OnContextMenuOpening()
        {
            _returnToSourceMenu.IsVisible = CanReturnToSource();
            _retryMessageMenu.IsVisible = CanRetryMessage();
            _copyMessageIdMenu.IsEnabled = CanCopyMessageId();
            _copyHeadersMenu.IsEnabled = CanCopyHeaders();
            NotifyPropertiesChanged();
        }

        public new IShellViewModel Parent { get { return (IShellViewModel) base.Parent; } }

        public ISearchBarViewModel SearchBar { get; private set; }

        public IObservableCollection<StoredMessage> SelectedRows { get; private set; }

        public IObservableCollection<StoredMessage> Rows { get; private set; } 

        public StoredMessage FocusedRow { get; set; }

        public Queue SelectedQueue { get; private set; }
		
        public bool WorkInProgress { get { return _workCount > 0 && !Parent.AutoRefresh; } }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public void ReturnToSource()
        {
            _errorHeaderDisplay.ReturnToSource();
        }

        public async void RetryMessage()
        {
            _eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", FocusedRow.SendingEndpoint));
            var msg = FocusedRow;
            await _serviceControl.RetryMessage(FocusedRow.Id);
            Rows.Remove(msg);
            _eventAggregator.Publish(new WorkFinished());
        }

        public void CopyMessageId()
        {
            _clipboard.CopyTo(_serviceControl.GetUri(FocusedRow).ToString());
        }

        public void CopyHeaders()
        {
            _clipboard.CopyTo(_generalHeaderDisplay.HeaderContent);
        }

        public bool CanRetryMessage()
        {
            return FocusedRow != null &&
                   (FocusedRow.Status == MessageStatus.Failed || 
                    FocusedRow.Status == MessageStatus.RepeatedFailure);
        }

        public bool CanReturnToSource()
        {
            return _errorHeaderDisplay.CanReturnToSource();
        }

        public bool CanCopyHeaders()
        {
            return !_generalHeaderDisplay.HeaderContent.IsEmpty();
        }

        public bool CanCopyMessageId()
        {
            return FocusedRow != null;
        }

        public async void OnFocusedRowChanged()
        {
            if (_lockUpdate) return;

            await LoadMessageBody();

            _eventAggregator.Publish(new SelectedMessageChanged(FocusedRow));

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
            _eventAggregator.Publish(new WorkStarted("Loading {0} messages...", endpoint == null ? "all" : endpoint.Address));

            if (orderBy != null)
            {
                _lastSortColumn = orderBy;
                _lastSortOrderAscending = ascending;
            }

            PagedResult<StoredMessage> pagedResult;

            if(endpoint != null)
            {
                pagedResult = await _serviceControl.GetAuditMessages(endpoint,
                                                                     pageIndex: pageIndex,
                                                                     searchQuery: searchQuery,
                                                                     orderBy: _lastSortColumn,
                                                                     ascending: _lastSortOrderAscending);
            }
            else if (!searchQuery.IsEmpty())
            {
                pagedResult = await _serviceControl.Search(pageIndex: pageIndex,
                                                           searchQuery: searchQuery,
                                                           orderBy: _lastSortColumn,
                                                           ascending: _lastSortOrderAscending);
            }
            else
            {
                pagedResult = await _serviceControl.Search(pageIndex: pageIndex,
                                                           searchQuery: null,
                                                           orderBy: _lastSortColumn,
                                                           ascending: _lastSortOrderAscending);
            }

            TryRebindMessageList(pagedResult);

            SearchBar.IsVisible = true;
            SearchBar.SetupPaging(new PagedResult<StoredMessage>
            {
                CurrentPage = pagedResult.CurrentPage,
                TotalCount = pagedResult.TotalCount,
                Result = pagedResult.Result,
            });

            _eventAggregator.Publish(new WorkFinished());
        }

        public string GetCriticalTime(StoredMessage msg)
        {
            if (msg != null && msg.Statistics != null)
                return msg.Statistics.ElapsedCriticalTime;

            return string.Empty;
        }

        public string GetProcessingTime(StoredMessage msg)
        {
            if (msg != null && msg.Statistics != null)
                return msg.Statistics.ElapsedProcessingTime;

            return string.Empty;
        }

        public MessageErrorInfo GetMessageErrorInfo()
        {
            return new MessageErrorInfo();
        }

        public MessageErrorInfo GetMessageErrorInfo(StoredMessage msg)
        {
            return new MessageErrorInfo(msg.Status);
        }

        public void Handle(WorkStarted @event)
        {
            _workCount++;
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public void Handle(WorkFinished @event)
        {
            if (_workCount > 0)
            {
                _workCount--;
                NotifyOfPropertyChange(() => WorkInProgress);
            }
        }

        public void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }

        public void Handle(AsyncOperationFailed message)
        {
            _workCount = 0;
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public void Handle(MessageStatusChanged message)
        {
            var msg = Rows.FirstOrDefault(x => x.MessageId == message.MessageId);
            if (msg != null)
            {
                msg.Status = MessageStatus.RetryIssued;
            }
        }

        public async void OnSelectedExplorerItemChanged()
        {
            var queueNode = SelectedExplorerItem.As<QueueExplorerItem>();
            if (queueNode != null)
            {
                SelectedQueue = queueNode.Queue;
            }

            await RefreshMessages();

            NotifyPropertiesChanged();
        }

        private void TryRebindMessageList(PagedResult<StoredMessage> pagedResult)
        {
            try
            {
                _lockUpdate = !ShouldUpdateMessages(pagedResult);

                using (new GridSelectionPreserver<StoredMessage>(this))
                using (new GridFocusedRowPreserver<StoredMessage>(this))
                {
                    Rows.Clear();
                    Rows.AddRange(pagedResult.Result);
                }
            }
            finally
            {
                _lockUpdate = false;
            }

            AutoSelectFirstRow();
            AutoFocusFirstRow();
        }

        private void AutoFocusFirstRow()
        {
            if (FocusedRow == null && Rows.Count > 0)
            {
                FocusedRow = Rows[0];
            }
        }

        private void AutoSelectFirstRow()
        {
            if (SelectedRows.Count == 0 && Rows.Count > 0)
            {
                SelectedRows.Add(Rows[0]);
            }
        }

        private bool ShouldUpdateMessages(PagedResult<StoredMessage> pagedResult)
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

        private static bool ShouldUpdateMessage(StoredMessage focusedMessage, StoredMessage newMessage)
        {
            return newMessage == null || newMessage.DisplayPropertiesChanged(focusedMessage);
        }

        private async Task LoadMessageBody()
        {
            if (FocusedRow == null) return;

            _eventAggregator.Publish(new WorkStarted("Loading message body..."));

            var body = await _serviceControl.GetBody(FocusedRow.BodyUrl);
            
            FocusedRow.Body = body;

            _eventAggregator.Publish(new WorkFinished());
        }

        private void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => SelectedExplorerItem);
            SearchBar.NotifyPropertiesChanged();
        }
    }
}