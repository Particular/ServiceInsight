using ServiceInsight.MessageList;

namespace ServiceInsight.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;

    public class RetryMessageCommand : BaseCommand
    {
        readonly IEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly MessageListViewModel parent;

        public RetryMessageCommand(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier, 
            MessageListViewModel parent)
        {
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.parent = parent;
        }

        public override bool CanExecute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
            {
                return false;
            }

            return message.Status == MessageStatus.Failed ||
                   message.Status == MessageStatus.RepeatedFailure ||
                   message.Status == MessageStatus.ArchivedFailure;
        }

        public override async void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
            {
                return;
            }

            using (workNotifier.NotifyOfWork($"Retrying to send selected error message {message.SendingEndpoint}"))
            {
                if (parent.ServiceControl != null)
                {
                    await parent.ServiceControl.RetryMessage(message.Id, message.InstanceId);
                    message.Status = MessageStatus.RetryIssued;
                    await eventAggregator.PublishOnUIThreadAsync(new RetryMessage {Id = message.Id});
                }
            }

            OnCanExecuteChanged();
        }
    }
}