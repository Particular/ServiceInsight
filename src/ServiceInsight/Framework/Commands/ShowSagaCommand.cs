namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;

    class ShowSagaCommand : BaseCommand
    {
        private readonly IEventAggregator eventAggregator;

        public ShowSagaCommand(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override bool CanExecute(object parameter)
        {
            var message = parameter as StoredMessage;
            return message != null;
        }

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
            {
                return;
            }

            eventAggregator.Publish(new SelectedMessageChanged(message));
            eventAggregator.Publish(SwitchToSagaWindow.Instance);
        }
    }
}