using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using ExceptionHandler;
using NServiceBus.Profiler.Desktop.Core;
using NServiceBus.Profiler.Desktop.Core.UI;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ExtensionMethods;
using NServiceBus.Profiler.Desktop.MessageProperties;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.ScreenManager;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Desktop.ServiceControl;
using NServiceBus.Profiler.Desktop.Shell;
using NServiceBus.Profiler.Desktop.Shell.Menu;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageListViewModel : Conductor<IScreen>.Collection.AllActive, IMessageListViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private readonly IServiceControl _serviceControl;
        private readonly IQueueManagerAsync _asyncQueueManager;
        private readonly IErrorHeaderViewModel _errorHeaderDisplay;
        private readonly IGeneralHeaderViewModel _generalHeaderDisplay;
        private readonly IClipboard _clipboard;
        private readonly IMenuItem _returnToSourceMenu;
        private readonly IMenuItem _retryMessageMenu;
        private readonly IMenuItem _copyMessageIdMenu;
        private readonly IMenuItem _copyHeadersMenu;
        private IMessageListView _view;
        private string _lastSortColumn;
        private bool _lastSortOrderAscending;
        private int _workCount;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            IServiceControl serviceControl,
            IQueueManagerAsync asyncQueueManager,
            ISearchBarViewModel searchBarViewModel,
            IErrorHeaderViewModel errorHeaderDisplay,
            IGeneralHeaderViewModel generalHeaderDisplay,
            IClipboard clipboard)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _serviceControl = serviceControl;
            _asyncQueueManager = asyncQueueManager;
            _errorHeaderDisplay = errorHeaderDisplay;
            _generalHeaderDisplay = generalHeaderDisplay;
            _clipboard = clipboard;

            SearchBar = searchBarViewModel;
            Items.Add(SearchBar);

            _returnToSourceMenu = new MenuItem("Return To Source", new RelayCommand(ReturnToSource, CanReturnToSource), Properties.Resources.MessageReturn);
            _retryMessageMenu = new MenuItem("Retry Message", new RelayCommand(RetryMessage, CanRetryMessage), Properties.Resources.MessageReturn);
            _copyMessageIdMenu = new MenuItem("Copy Message Identifier", new RelayCommand(CopyMessageId, CanCopyMessageId));
            _copyHeadersMenu = new MenuItem("Copy Headers", new RelayCommand(CopyHeaders, CanCopyHeaders));

            Messages = new BindableCollection<MessageInfo>();
            SelectedMessages = new BindableCollection<MessageInfo>();
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

        public IObservableCollection<MessageInfo> Messages { get; private set; }

        public IObservableCollection<MessageInfo> SelectedMessages { get; private set; }

        public ISearchBarViewModel SearchBar { get; private set; }

        public MessageInfo FocusedMessage { get; set; }

        public StoredMessage StoredMessage
        {
            get { return FocusedMessage as StoredMessage; }
        }

        public Queue SelectedQueue { get; private set; }
		
        public bool WorkInProgress { get { return _workCount > 0 && !Parent.AutoRefresh; } }

        public ExplorerItem SelectedExplorerItem { get; private set; }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (IMessageListView) view;
        }

        public void ReturnToSource()
        {
            _errorHeaderDisplay.ReturnToSource();
        }

        public async void RetryMessage()
        {
            _eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", StoredMessage.OriginatingEndpoint));
            var msg = (StoredMessage)FocusedMessage;
            await _serviceControl.RetryMessage(FocusedMessage.Id);
            Messages.Remove(msg);
            _eventAggregator.Publish(new WorkFinished());
        }

        public void CopyMessageId()
        {
            var msg = FocusedMessage;
            _clipboard.CopyTo(msg.Id);
        }

        public void CopyHeaders()
        {
            _clipboard.CopyTo(_generalHeaderDisplay.HeaderContent);
        }

        public bool CanRetryMessage()
        {
            return StoredMessage != null &&
                    (StoredMessage.Status == MessageStatus.Failed || StoredMessage.Status == MessageStatus.RepeatedFailure);
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
            return FocusedMessage != null;
        }

        public void OnFocusedMessageChanged() 
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

        public async Task RefreshMessages(string orderBy = null, bool ascending = false)
        {
            var queueNode = SelectedExplorerItem.As<QueueExplorerItem>();
            if(queueNode != null)
            {
                await RefreshQueue(queueNode.Queue);
                return;
            }

            var queueServer = SelectedExplorerItem.As<QueueServerExplorerItem>();
            if (queueServer != null)
            {
                //TODO: Refresh all queues
                return;
            }

            var serviceControl = SelectedExplorerItem.As<ServiceControlExplorerItem>();
            if (serviceControl != null)
            {
                await RefreshEndpoint(searchQuery: SearchBar.SearchQuery,
                                      endpoint: null,
                                      orderBy: orderBy,
                                      ascending: ascending);
            }
            
            var endpointNode = SelectedExplorerItem.As<AuditEndpointExplorerItem>();
            if (endpointNode != null)
            {
                await RefreshEndpoint(searchQuery: SearchBar.SearchQuery,
                                      endpoint: endpointNode.Endpoint,
                                      orderBy: orderBy,
                                      ascending: ascending);
            }
        }

        public async Task RefreshEndpoint(Endpoint endpoint, int pageIndex = 1, string searchQuery = null, string orderBy = null, bool ascending = false)
        {
            _eventAggregator.Publish(new WorkStarted("Loading {0} messages...", endpoint == null ? "all" : endpoint.Address));

            if (orderBy != null)
            {
                _lastSortColumn = orderBy;
                _lastSortOrderAscending = ascending;
            }

            var pagedResult = new PagedResult<StoredMessage>();

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
            _eventAggregator.Publish(new WorkStarted("Loading {0} messages...", queue));

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

        public async Task DeleteSelectedMessages()
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

        public async Task PurgeQueue()
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
            SearchBar.NotifyPropertiesChanged();
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
            var msg = Messages.OfType<StoredMessage>().FirstOrDefault(x => x.MessageId == message.MessageId);
            if (msg != null)
            {
                msg.Status = MessageStatus.RetryIssued;
            }
        }
    }
}