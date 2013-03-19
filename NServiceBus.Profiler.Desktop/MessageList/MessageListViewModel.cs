using System.Threading.Tasks;
using System.Windows;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Core.Management;
using NServiceBus.Profiler.Desktop.Explorer.EndpointExplorer;
using NServiceBus.Profiler.Desktop.ScreenManager;
using System.Linq;
using NServiceBus.Profiler.Desktop.Search;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageListViewModel : Screen, IMessageListViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private readonly IManagementService _managementService;
        private readonly IQueueManagerAsync _asyncQueueManager;
        private readonly IEndpointConnectionProvider _endpointConnection;

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
            Messages = new BindableCollection<MessageInfo>();
            SelectedMessages = new BindableCollection<MessageInfo>();
        }

        public virtual IObservableCollection<MessageInfo> Messages { get; private set; }

        public virtual IObservableCollection<MessageInfo> SelectedMessages { get; private set; }

        public virtual ISearchBarViewModel SearchBar { get; private set; }

        public virtual MessageInfo FocusedMessage { get; set; }

        public virtual Queue SelectedQueue { get; set; }

        public virtual void OnFocusedMessageChanged() 
        {
            //TODO: Block the view when loading the message content
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

        public virtual void OnSelectedQueueChanged()
        {
            RefreshMessages();
        }

        public virtual async void RefreshMessages()
        {
            //TODO: Support context specific refresh (e.g. refreshing endpoints or queues)
            var queue = SelectedQueue;
            if (queue == null)
                return;

            _eventAggregator.Publish(new WorkStarted("Loading queue messages..."));

            var messages = await _asyncQueueManager.GetMessages(queue);

            Messages.Clear();
            Messages.AddRange(messages);

            //TODO: Disable paging when loading queue messages
            //SearchBar.SetupPaging(Messages);

            _eventAggregator.Publish(new WorkFinished());

            _eventAggregator.Publish(new QueueMessageCountChanged(SelectedQueue, Messages.Count));
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

        public virtual void Handle(SelectedQueueChanged @event)
        {
            SelectedQueue = @event.SelectedQueue;
        }

        public virtual void DeleteSelectedMessages()
        {
            if (SelectedQueue == null || SelectedMessages.Count <= 0)
                return;

            var confirmation = string.Format("{0} message(s) are being remove from {1}. Continue?",
                                             SelectedMessages.Count, SelectedQueue.Address);
            var result = _windowManager.ShowMessageBox(confirmation, "Warning", MessageBoxButton.OKCancel,
                                                       MessageBoxImage.Question);
            if (result != MessageBoxResult.OK)
                return;

            foreach (var msg in SelectedMessages)
            {
                _asyncQueueManager.DeleteMessage(SelectedQueue, msg);
            }

            RefreshMessages();
        }

        public virtual void PurgeQueue()
        {
            if (SelectedQueue == null)
                return;

            var confirmation = string.Format("All the messages in {0} will be removed. Continue?", SelectedQueue.Address);
            var dialogTitle = string.Format("Purge Queue: {0}", SelectedQueue.Address.Queue);
            var result = _windowManager.ShowMessageBox(confirmation, dialogTitle, MessageBoxButton.OKCancel, MessageBoxImage.Question, defaultChoice: MessageChoice.Cancel);
            
            if (result != MessageBoxResult.OK)
                return;

            _asyncQueueManager.Purge(SelectedQueue);
            RefreshMessages();
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
                await RefreshQueueMessages(queue);
            }

            //TODO: Refresh message count on error and audit queue
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

        public async void Handle(LoadAuditMessages @event)
        {
            _eventAggregator.Publish(new WorkStarted("Loading audit messages..."));

            var pagedResult = await _managementService.GetAuditMessages(_endpointConnection.ServiceUrl, 
                                                                        @event.Endpoint, 
                                                                        pageIndex: @event.PageIndex, 
                                                                        searchQuery: @event.SearchQuery);
 
            Messages.Clear();
            Messages.AddRange(pagedResult.Result);
            SearchBar.SetupPaging(pagedResult);
            
            _eventAggregator.Publish(new WorkFinished());
        }

        public async void Handle(ErrorQueueSelected message)
        {
            _eventAggregator.Publish(new WorkStarted("Loading failed messages..."));

            var messages = await _managementService.GetErrorMessages(_endpointConnection.ServiceUrl);

            Messages.Clear();
            Messages.AddRange(messages.Result);

            _eventAggregator.Publish(new WorkFinished());
        }

        public void Handle(WorkStarted message)
        {
            WorkInProgress = true;
        }

        public void Handle(WorkFinished message)
        {
            WorkInProgress = false;
        }

        public bool WorkInProgress { get; private set; }
    }
}