namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.Search;

    class SearchByMessageIDCommand : BaseCommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly SearchBarViewModel searchBar;

        public SearchByMessageIDCommand(IEventAggregator eventAggregator, SearchBarViewModel searchBar)
        {
            this.eventAggregator = eventAggregator;
            this.searchBar = searchBar;
        }

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            searchBar.Search(performSearch: true, searchQuery: message.MessageId);
            eventAggregator.Publish(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }
    }
}