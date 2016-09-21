namespace ServiceInsight.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;
    using ServiceInsight.MessageList;

    class ShowSagaCommand : BaseCommand
    {
        readonly IRxEventAggregator eventAggregator;
        readonly MessageSelectionContext selection;

        public ShowSagaCommand(IRxEventAggregator eventAggregator, MessageSelectionContext selectionContext)
        {
            this.eventAggregator = eventAggregator;
            selection = selectionContext;
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