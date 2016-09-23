namespace ServiceInsight.Framework.Commands
{
    using Events;
    using Models;
    using ServiceControl;

    public class RetryMessageCommand : BaseCommand
    {
        readonly IRxEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly IServiceControl serviceControl;

        public RetryMessageCommand(IRxEventAggregator eventAggregator, IWorkNotifier workNotifier, IServiceControl serviceControl)
        {
            this.workNotifier = workNotifier;
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
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

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
            {
                return;
            }

            using (workNotifier.NotifyOfWork($"Retrying to send selected error message {message.SendingEndpoint}"))
            {
                serviceControl.RetryMessage(message.Id);
                eventAggregator.Publish(new RetryMessage { Id = message.Id });
            }

            message.Status = MessageStatus.RetryIssued;

            OnCanExecuteChanged();
        }
    }
}