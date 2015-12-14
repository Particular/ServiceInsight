namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;
    using Particular.ServiceInsight.Desktop.MessageList;

    class ShowSagaCommand : BaseCommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly MessageSelectionContext selection;

        public ShowSagaCommand(IEventAggregator eventAggregator, MessageSelectionContext selectionContext)
        {
            this.eventAggregator = eventAggregator;
            this.selection = selectionContext;
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

            selection.SelectedMessage = message;
            eventAggregator.Publish(SwitchToSagaWindow.Instance);
        }
    }
}