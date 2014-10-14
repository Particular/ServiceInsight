namespace Particular.ServiceInsight.Desktop.Common
{
    using Caliburn.Micro;
    using Events;
    using Models;
    using Search;

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