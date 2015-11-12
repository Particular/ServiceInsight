namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;

    public class ChangeSelectedMessageCommand : BaseCommand
    {
        readonly IEventAggregator eventAggregator;

        public ChangeSelectedMessageCommand(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override bool CanExecute(object parameter)
        {
            var msg = parameter as StoredMessage;
            return msg != null;
        }

        public override void Execute(object parameter)
        {
            eventAggregator.Publish(new SelectedMessageChanged(parameter as StoredMessage));
        }
    }
}