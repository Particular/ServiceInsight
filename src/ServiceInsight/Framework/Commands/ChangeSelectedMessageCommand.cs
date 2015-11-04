namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;

    class ChangeSelectedMessageCommand : BaseCommand
    {
        readonly IEventAggregator eventAggregator;

        public ChangeSelectedMessageCommand(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override void Execute(object parameter)
        {
            eventAggregator.Publish(new SelectedMessageChanged(parameter as StoredMessage));
        }
    }
}