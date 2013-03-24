using System.Threading.Tasks;
using System.Windows;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Explorer;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.Explorer.QueueExplorer;
using NServiceBus.Profiler.Desktop.ScreenManager;
using System.Linq;
using NServiceBus.Profiler.Desktop.Search;
using NServiceBus.Profiler.Common.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageListViewModel : Conductor<IScreen>.Collection.AllActive, IMessageListViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private readonly IManagementService _managementService;
        private readonly IQueueManagerAsync _asyncQueueManager;
        private readonly IEndpointConnectionProvider _endpointConnection;
        private int _workCount;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            IManagementService managementService,
            IQueueManagerAsync asyncQueueManager,
            IEndpointConnectionProvider endpointConnection,
            ISearchBarViewModel searchBarViewModel)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _managementService = managementService;
            _asyncQueueManager = asyncQueueManager;
            _endpointConnection = endpointConnection;

            SearchBar = searchBarViewModel;

            Items.Add(SearchBar);
            Messages = new BindableCollection<MessageInfo>();
            SelectedMessages = new BindableCollection<MessageInfo>();
        }

        public virtual IObservableCollection<MessageInfo> Messages { get; private set; }

        public virtual IObservableCollection<MessageInfo> SelectedMessages { get; private set; }

        public virtual ISearchBarViewModel SearchBar { get; private set; }

        public virtual MessageInfo FocusedMessage { get; set; }

        public virtual Queue SelectedQueue { get; private set; }

        public virtual bool WorkInProgress { get { return _workCount > 0; } }

        public virtual ExplorerItem SelectedExplorerItem { get; private set; }

        public virtual void OnFocusedMessageChanged() 
        {
            _eventAggregator.Publish(new SelectedMessageChanged(FocusedMessage));

            var loadedMessage = FocusedMessage as StoredMessage;
            if (loadedMessage != null)
            {
                _eventAggregator.Publish(new MessageBodyLoaded(loadedMessage));
                return;
            }

            var queueMessage = FocusedMessage as MessageInfo;
            if (queueMessage != null && SelectedQueue != null)
            {
                LoadQueueMessage(SelectedQueue, FocusedMessage.Id);
            }
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

        public async Task RefreshMessages()
        {
            var endpointNode = SelectedExplorerItem.As<AuditEndpointExplorerItem>();
            if (endpointNode != null)
            {
                await RefreshEndpoint(endpointNode.Endpoint);
            }

            var queueNode = SelectedExplorerItem.As<QueueExplorerItem>();
            if(queueNode != null)
            {
                await RefreshQueue(queueNode.Queue);
            }
        }

        public async Task RefreshEndpoint(Endpoint endpoint, int pageIndex = 1, string searchQuery = null)
        {
            _eventAggregator.Publish(new WorkStarted(string.Format("Loading {0} messages...", endpoint)));

            var pagedResult = await _managementService.GetAuditMessages(_endpointConnection.ServiceUrl,
                                                                        endpoint,
                                                                        pageIndex: pageIndex,
                                                                        searchQuery: searchQuery);

            Messages.Clear();
            Messages.AddRange(pagedResult.Result);

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

            await RefreshQueueMessages(queue);
            await RefreshQueueMessageCount(queue);

            _eventAggregator.Publish(new WorkFinished());
        }

        private async Task RefreshQueueMessageCount(Queue queue)
        {
            var messages = await _asyncQueueManager.GetMessages(queue);

            Messages.Clear();
            Messages.AddRange(messages);

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

        public string GetCriticalTime(StoredMessage msg)
        {
            if (msg.Statistics != null)
            {
                var criticalTime = msg.Statistics.CriticalTime;
                if (criticalTime.TotalSeconds < 1.0)
                    return string.Format("{0}ms", criticalTime.Milliseconds);

                if (criticalTime.TotalMinutes < 1.0)
                    return string.Format("{0}s", criticalTime.Seconds);
                
                if (criticalTime.TotalHours < 1.0)
                    return string.Format("{0}m {1:D2}s", criticalTime.Minutes, criticalTime.Seconds);
                
                return string.Format("{0}h {1:D2}m {2:D2}s", (int)criticalTime.TotalHours, criticalTime.Minutes, criticalTime.Seconds);
            }

            return string.Empty;
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

        public void Handle(MessageRemovedFromQueue @event)
        {
            if (Messages.Contains(@event.Message))
            {
                Messages.Remove(@event.Message);
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
        }

        public virtual void Handle(SelectedExplorerItemChanged @event)
        {
            SelectedExplorerItem = @event.SelectedExplorerItem;
        }
    }
}