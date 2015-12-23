namespace ServiceInsight.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;
    using Search;

    public class SearchByMessageIDCommand : BaseCommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly SearchBarViewModel searchBar;

        public SearchByMessageIDCommand(IEventAggregator eventAggregator, SearchBarViewModel searchBar)
        {
            this.eventAggregator = eventAggregator;
            this.searchBar = searchBar;
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
                return;

            searchBar.Search(performSearch: true, searchQuery: message.MessageId);
            eventAggregator.PublishOnUIThread(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }
    }
}