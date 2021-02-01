namespace ServiceInsight.Framework.Commands
{
    using Caliburn.Micro;
    using Events;
    using Models;
    using Search;

    public class SearchByMessageIDCommand : BaseCommand
    {
        readonly IEventAggregator eventAggregator;
        readonly SearchBarViewModel searchBar;

        public SearchByMessageIDCommand(IEventAggregator eventAggregator, SearchBarViewModel searchBar)
        {
            this.eventAggregator = eventAggregator;
            this.searchBar = searchBar;
        }

        public override bool CanExecute(object parameter)
        {
            return parameter is StoredMessage;
        }

        public override async void Execute(object parameter)
        {
            if (!(parameter is StoredMessage message))
            {
                return;
            }

            await searchBar.Search(performSearch: true, searchQuery: message.MessageId);
            eventAggregator.PublishOnUIThread(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }
    }
}