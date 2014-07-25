using System;

namespace Particular.ServiceInsight.Desktop.Common
{
    using System.Windows.Input;
    using Caliburn.Micro;
    using Events;
    using Models;
    using ServiceControl;

    class RetryMessageCommand : ICommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IServiceControl serviceControl;

        public RetryMessageCommand(IEventAggregator eventAggregator, IServiceControl serviceControl)
        {
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
        }

        public bool CanExecute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return false;

            return message.Status == MessageStatus.Failed ||
                   message.Status == MessageStatus.RepeatedFailure ||
                   message.Status == MessageStatus.ArchivedFailure;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            eventAggregator.Publish(new WorkStarted("Retrying to send selected error message {0}", message.SendingEndpoint));
            serviceControl.RetryMessage(message.Id);
            eventAggregator.Publish(new RetryMessage { MessageId = message.MessageId });
            eventAggregator.Publish(new WorkFinished());

            message.Status = MessageStatus.RetryIssued;

            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}