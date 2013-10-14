using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.Management;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Search;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NServiceBus.Profiler.Desktop.Shell;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageListViewModel : Conductor<IScreen>.Collection.AllActive, IMessageListViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private readonly IManagementService _managementService;
        private readonly IQueueManagerAsync _asyncQueueManager;
        private readonly IErrorHeaderViewModel _errorHeaderDisplay;
        private readonly IGeneralHeaderViewModel _generalHeaderDisplay;
        private readonly IClipboard _clipboard;
        private readonly IStatusBarManager _statusBar;
        private IMessageListView _view;
        private string _lastSortColumn;
        private bool _lastSortOrderAscending;
        private int _workCount;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            IManagementService managementService,
            IQueueManagerAsync asyncQueueManager,
            ISearchBarViewModel searchBarViewModel,
            IErrorHeaderViewModel errorHeaderDisplay,
            IGeneralHeaderViewModel generalHeaderDisplay,
            IClipboard clipboard,
            IStatusBarManager statusBar)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _managementService = managementService;
            _asyncQueueManager = asyncQueueManager;
            _errorHeaderDisplay = errorHeaderDisplay;
            _generalHeaderDisplay = generalHeaderDisplay;
            _clipboard = clipboard;
            _statusBar = statusBar;

            SearchBar = searchBarViewModel;

            Items.Add(SearchBar);
            Messages = new BindableCollection<MessageInfo>();
            SelectedMessages = new BindableCollection<MessageInfo>();
            ContextMenuItems = new BindableCollection<ContextMenuModel>();
        }

        public virtual IObservableCollection<ContextMenuModel> ContextMenuItems { get; private set; }

        public virtual IObservableCollection<MessageInfo> Messages { get; private set; }

        public virtual IObservableCollection<MessageInfo> SelectedMessages { get; private set; }

        public virtual ISearchBarViewModel SearchBar { get; private set; }

        public virtual MessageInfo FocusedMessage { get; set; }

        public virtual StoredMessage StoredMessage
        {
            get { return FocusedMessage as StoredMessage; }
        }

        public virtual Queue SelectedQueue { get; private set; }

        public virtual bool WorkInProgress { get { return _workCount > 0; } }

        public virtual ExplorerItem SelectedExplorerItem { get; private set; }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IMessageListView) view;
            _view.SetupContextMenu();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            ContextMenuItems.Add(new ContextMenuModel(this, "ReturnToSource", "Return To Source", Properties.Resources.MessageReturn));
            ContextMenuItems.Add(new ContextMenuModel(this, "RetryMessage", "Retry Message", Properties.Resources.MessageReturn));
            ContextMenuItems.Add(new ContextMenuModel(this, "CopyMessageId", "Copy Message Identifier"));
            ContextMenuItems.Add(new ContextMenuModel(this, "CopyHeaders", "Copy Headers"));
        }

        public virtual void ReturnToSource()
        {
            _errorHeaderDisplay.ReturnToSource();
        }

        public virtual async void RetryMessage()
        {
            _statusBar.SetSuccessStatusMessage("Retrying to send selected error message {0}", StoredMessage.OriginatingEndpoint);
            var msg = (StoredMessage)FocusedMessage;
            await _managementService.RetryMessage(FocusedMessage.Id);
            Messages.Remove(msg);
            _statusBar.Done();
        }

        public virtual void CopyMessageId()
        {
            var msg = FocusedMessage;
            _clipboard.CopyTo(msg.Id);
        }

        public virtual void CopyHeaders()
        {
            //_generalHeaderDisplay.CopyHeaderInfo();
        }

        public bool CanRetryMessage
        {
            get
            {
                return StoredMessage != null &&
                       (StoredMessage.Status == MessageStatus.Failed || StoredMessage.Status == MessageStatus.RepeatedFailures);
            }
        }

        public bool CanReturnToSource
        {
            get { return _errorHeaderDisplay.CanReturnToSource(); }
        }

        public bool CanCopyHeaders
        {
            get
            {
                return false;
            } //_generalHeaderDisplay.CanCopyHeaderInfo(); }
        }

        public bool CanCopyMessageId
        {
            get { return FocusedMessage != null; }
        }

        public virtual void OnFocusedMessageChanged() 
        {
            _eventAggregator.Publish(new SelectedMessageChanged(FocusedMessage));

            if (StoredMessage != null)
            {
                _eventAggregator.Publish(new MessageBodyLoaded(StoredMessage));
                return;
            }

            var queueMessage = FocusedMessage as MessageInfo;
            if (queueMessage != null && SelectedQueue != null)
            {
                LoadQueueMessage(SelectedQueue, FocusedMessage.Id);
            }

            NotifyPropertiesChanged();
        }

        private void LoadQueueMessage(Queue queue, string selectedMessageId)
        {
            var msg = _asyncQueueManager.GetMessageBody(queue, selectedMessageId);
            if (msg == null)
            {
                FocusedMessage.IsDeleted = true;
                NotifyOfPropertyChange(() => FocusedMessage);
            }
            _eventAggregator.Publish(new MessageBodyLoaded(msg));
        }

        public async Task RefreshMessages(string columnName = null, bool ascending = false)
        {
            var endpointNode = SelectedExplorerItem.As<AuditEndpointExplorerItem>();
            if (endpointNode != null)
            {
                await RefreshEndpoint(endpointNode.Endpoint, 
                                      searchQuery: SearchBar.SearchQuery,
                                      orderBy: columnName, 
                                      ascending: ascending);
            }

            var queueNode = SelectedExplorerItem.As<QueueExplorerItem>();
            if(queueNode != null)
            {
                await RefreshQueue(queueNode.Queue);
            }
        }

        public async Task RefreshEndpoint(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false)
        {
            _eventAggregator.Publish(new WorkStarted(string.Format("Loading {0} messages...", endpoint)));

            if (orderBy != null)
            {
                _lastSortColumn = orderBy;
                _lastSortOrderAscending = ascending;
            }

            var pagedResult = await _managementService.GetAuditMessages(endpoint,
                                                                        pageIndex: pageIndex,
                                                                        searchQuery: searchQuery,
                                                                        orderBy: _lastSortColumn,
                                                                        ascending: _lastSortOrderAscending);
            using (new GridSelectionPreserver(_view))
            {
                Messages.Clear();
                Messages.AddRange(pagedResult.Result);
            }

            SearchBar.IsVisible = true;
            SearchBar.SetupPaging(new PagedResult<MessageInfo>
            {
                CurrentPage = pagedResult.CurrentPage,
                TotalCount = pagedResult.TotalCount,
                Result = pagedResult.Result.Cast<MessageInfo>().ToList(),
            });

            _eventAggregator.Publish(new WorkFinished());
        }

        public async Task RefreshQueue(Queue queue)
        {
            _eventAggregator.Publish(new WorkStarted(string.Format("Loading {0} messages...", queue)));

            //await RefreshQueueMessages(queue); //TODO: Do we even need this anymore?? Seems to be doing the same thing as the other!
            await RefreshQueueMessageCount(queue);

            _eventAggregator.Publish(new WorkFinished());
        }

        private async Task RefreshQueueMessageCount(Queue queue)
        {
            var messages = await _asyncQueueManager.GetMessages(queue);

            using (new GridSelectionPreserver(_view))
            {
                Messages.Clear();
                Messages.AddRange(messages);
            }

            SearchBar.IsVisible = false;
            SearchBar.SetupPaging(new PagedResult<MessageInfo>
            {
                TotalCount = messages.Count,
                CurrentPage = 1,
                Result = messages
            });

            _eventAggregator.Publish(new QueueMessageCountChanged(SelectedQueue, Messages.Count));
        }

        private async Task RefreshQueueMessages(Queue queue)
        {
            var messages = await _asyncQueueManager.GetMessages(queue);

            var existingMessages = Messages.Select(x => x.Id).ToArray();
            var updatedMessages = messages.Select(x => x.Id).ToArray();
            var removedMessages = existingMessages.Union(updatedMessages).Except(updatedMessages).ToList();
            var newMessages = updatedMessages.Except(existingMessages).ToList();

            using (new GridSelectionPreserver(_view))
            {
                foreach (var removedMessageId in removedMessages)
                {
                    var message = Messages.SingleOrDefault(m => m.Id == removedMessageId);
                    if (message != null)
                    {
                        Messages.Remove(message);
                    }
                }

                foreach (var newMessagesId in newMessages)
                {
                    var message = messages.FirstOrDefault(m => m.Id == newMessagesId);
                    Messages.Add(message);
                }
            }
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

        public virtual async Task DeleteSelectedMessages()
        {
            if (SelectedQueue == null || SelectedMessages.Count <= 0)
                return;

            var confirmation = string.Format("{0} {1} being removed from {2}. Continue?",
                                             "message".PluralizeWord(SelectedMessages.Count), 
                                             SelectedMessages.Count.PluralizeVerb(),
                                             SelectedQueue.Address);

            var result = _windowManager.ShowMessageBox(confirmation, "Warning", MessageBoxButton.OKCancel,
                                                       MessageBoxImage.Question);
            if (result != MessageBoxResult.OK)
                return;

            foreach (var msg in SelectedMessages)
            {
                _asyncQueueManager.DeleteMessage(SelectedQueue, msg);
            }

            await RefreshMessages();
        }

        public virtual async Task PurgeQueue()
        {
            if (SelectedQueue == null)
                return;

            var confirmation = string.Format("All the messages in {0} will be removed. Continue?", SelectedQueue.Address);
            var dialogTitle = string.Format("Purge Queue: {0}", SelectedQueue.Address.Queue);
            var result = _windowManager.ShowMessageBox(confirmation, dialogTitle, MessageBoxButton.OKCancel, MessageBoxImage.Question, defaultChoice: MessageChoice.Cancel);

            if (result != MessageBoxResult.OK)
                return;

            _asyncQueueManager.Purge(SelectedQueue);
            await RefreshMessages();
        }

        public async void Handle(MessageRemovedFromQueue @event)
        {
            var queue = SelectedQueue;
            var msg = Messages.FirstOrDefault(x => x.Id == @event.Message.Id);

            if (msg != null)
            {
                Messages.Remove(msg);
                await RefreshQueueMessageCount(queue);
            }
        }

        public async void Handle(AutoRefreshBeat @event)
        {
            var queue = SelectedQueue;
            if (queue != null)
            {
                await RefreshMessages();
            }
        }
        
        public virtual void Handle(WorkStarted @event)
        {
            _workCount++;
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public virtual void Handle(WorkFinished @event)
        {
            if (_workCount > 0)
            {
                _workCount--;
                NotifyOfPropertyChange(() => WorkInProgress);
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

        private void NotifyPropertiesChanged()
        {
            NotifyOfPropertyChange(() => SelectedExplorerItem);
            NotifyOfPropertyChange(() => CanCopyHeaders);
            NotifyOfPropertyChange(() => CanCopyMessageId);
            NotifyOfPropertyChange(() => CanReturnToSource);
            NotifyOfPropertyChange(() => CanRetryMessage);
            SearchBar.NotifyPropertiesChanged();
        }

        public virtual void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }

        public virtual void Handle(AsyncOperationFailedEvent message)
        {
            _workCount = 0;
            NotifyOfPropertyChange(() => WorkInProgress);
        }

        public void Handle(MessageStatusChanged message)
        {
            var msg = Messages.OfType<StoredMessage>().FirstOrDefault(x => x.MessageId == message.MessageId);
            if (msg != null)
            {
                msg.Status = MessageStatus.RetryIssued;
            }
        }
    }
}