using System.Threading.Tasks;
using System.Windows;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Common.Events;
using NServiceBus.Profiler.Common.Models;
using NServiceBus.Profiler.Core;
using NServiceBus.Profiler.Desktop.ScreenManager;
using System.Linq;

namespace NServiceBus.Profiler.Desktop.MessageList
{
    public class MessageListViewModel : Screen, IMessageListViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManagerEx _windowManager;
        private readonly IQueueManagerAsync _asyncQueueManager;

        public MessageListViewModel(
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            IQueueManagerAsync asyncQueueManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _asyncQueueManager = asyncQueueManager;
            Messages = new BindableCollection<MessageInfo>();
            SelectedMessages = new BindableCollection<MessageInfo>();
        }

        public virtual IObservableCollection<MessageInfo> Messages { get; private set; }

        public virtual IObservableCollection<MessageInfo> SelectedMessages { get; private set; }

        public virtual MessageInfo FocusedMessage { get; set; }

        public virtual Queue SelectedQueue { get; set; }

        public virtual void OnFocusedMessageChanged()
        {
            _eventAggregator.Publish(new SelectedMessageChangedEvent(FocusedMessage));

            if (FocusedMessage != null && SelectedQueue != null)
            {
                var msg = _asyncQueueManager.GetMessageBody(SelectedQueue, FocusedMessage.Id);
                if (msg == MessageBody.Empty)
                {
                    FocusedMessage.IsDeleted = true;
                    NotifyOfPropertyChange(() => FocusedMessage);
                }
                _eventAggregator.Publish(new MessageBodyLoadedEvent(msg));
            }
        }

        public virtual void OnSelectedQueueChanged()
        {
            RefreshMessages();
        }

        public virtual async void RefreshMessages()
        {
            var queue = SelectedQueue;
            if (queue == null)
                return;

            _eventAggregator.Publish(new WorkStartedEvent());

            var messages = await _asyncQueueManager.GetMessages(queue);
            Messages.Clear();
            Messages.AddRange(messages);

            _eventAggregator.Publish(new WorkFinishedEvent());

            _eventAggregator.Publish(new QueueMessageCountChanged(SelectedQueue, Messages.Count));
        }

        public virtual void Handle(SelectedQueueChangedEvent @event)
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
            var result = _windowManager.ShowMessageBox(confirmation, "Warning", MessageBoxButton.OKCancel,
                                                       MessageBoxImage.Question);
            if (result != MessageBoxResult.OK)
                return;

            _asyncQueueManager.Purge(SelectedQueue);
            RefreshMessages();
        }

        public void Handle(MessageRemovedFromQueueEvent @event)
        {
            if (Messages.Contains(@event.Message))
            {
                Messages.Remove(@event.Message);
            }
        }

        public async void Handle(AutoRefreshBeatEvent @event)
        {
            var queue = SelectedQueue;
            if (queue != null)
            {
                await RefreshQueueMessages(queue);
            }
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
    }
}