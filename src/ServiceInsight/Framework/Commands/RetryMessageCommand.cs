namespace ServiceInsight.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;
    using ServiceControl;

    public class RetryMessageCommand : BaseCommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IServiceControl serviceControl;

        public RetryMessageCommand(IEventAggregator eventAggregator, IServiceControl serviceControl)
        {
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
        }

        public override bool CanExecute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return false;

            return message.Status == MessageStatus.Failed ||
                   message.Status == MessageStatus.RepeatedFailure ||
                   message.Status == MessageStatus.ArchivedFailure;
        }

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", message.SendingEndpoint));
            serviceControl.RetryMessage(message.Id);
            eventAggregator.Publish(new RetryMessage { Id = message.Id });
            eventAggregator.Publish(new WorkFinished());

            message.Status = MessageStatus.RetryIssued;

            OnCanExecuteChanged();
        }
    }
}