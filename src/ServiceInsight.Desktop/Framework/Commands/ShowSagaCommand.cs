namespace Particular.ServiceInsight.Desktop.Framework.Commands
{
    using System.Linq;
    using Caliburn.Micro;
    using Particular.ServiceInsight.Desktop.Events;
    using Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer;
    using Particular.ServiceInsight.Desktop.MessageList;
    using Particular.ServiceInsight.Desktop.Models;

    class ShowSagaCommand : BaseCommand
    {
        private readonly EndpointExplorerViewModel endpointExplorer;
        private readonly IEventAggregator eventAggregator;
        private readonly MessageListViewModel messageList;

        public ShowSagaCommand(IEventAggregator eventAggregator, EndpointExplorerViewModel endpointExplorer, MessageListViewModel messageList)
        {
            this.messageList = messageList;
            this.endpointExplorer = endpointExplorer;
            this.eventAggregator = eventAggregator;
        }

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
                return;

            if (messageList.Rows.All(r => r.Id != message.Id))
            {
                endpointExplorer.SelectedNode = endpointExplorer.ServiceControlRoot;
            }
            messageList.Focus(message);
            eventAggregator.Publish(SwitchToSagaWindow.Instance);
        }
    }
}