namespace Particular.ServiceInsight.Desktop.Common
{
    using System;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Events;
    using Models;
    using Search;

    class SearchByMessageIDCommand : ICommand
    {
        private readonly IEventAggregator eventAggregator;
        private readonly SearchBarViewModel searchBar;

        public SearchByMessageIDCommand(IEventAggregator eventAggregator, SearchBarViewModel searchBar)
        {
            this.eventAggregator = eventAggregator;
            this.searchBar = searchBar;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            searchBar.Search(performSearch: true, searchQuery: message.MessageId);
            eventAggregator.Publish(new RequestSelectingEndpoint(message.ReceivingEndpoint));
        }
    }
}