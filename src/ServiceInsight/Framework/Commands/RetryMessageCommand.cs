namespace ServiceInsight.Framework.Commands
{
    using Caliburn.Micro;
    using ServiceInsight.Framework.Events;
    using ServiceInsight.MessageList;
    using ServiceInsight.Models;

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
            if (!(parameter is StoredMessage message))
            {
                return false;
            }

            return message.Status == MessageStatus.Failed ||
                   message.Status == MessageStatus.RepeatedFailure ||
                   message.Status == MessageStatus.ArchivedFailure;
        }

        public override async void Execute(object parameter)
        {
            if (!(parameter is StoredMessage message))
            {
                return;
            }

            using (workNotifier.NotifyOfWork($"Retrying to send selected error message {message.SendingEndpoint}"))
            {
                if (parent.ServiceControl != null)
                {
                    await parent.ServiceControl.RetryMessage(message.Id, message.InstanceId);
                    message.Status = MessageStatus.RetryIssued;
                    await eventAggregator.PublishOnUIThreadAsync(new RetryMessage { Id = message.Id });
                }
            }

            OnCanExecuteChanged();
        }
    }
}