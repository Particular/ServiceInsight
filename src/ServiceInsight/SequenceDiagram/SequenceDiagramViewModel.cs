namespace ServiceInsight.SequenceDiagram
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Diagram;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.Search;
    using Particular.ServiceInsight.Desktop.ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IServiceControl serviceControl;
        readonly IEventAggregator eventAggregator;

        public SequenceDiagramViewModel(IServiceControl serviceControl, IClipboard clipboard,
            IEventAggregator eventAggregator,
            SearchBarViewModel searchBar)
        {
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            DiagramItems = new DiagramItemCollection();

            CopyConversationIDCommand = new CopyConversationIDCommand(clipboard);
            CopyMessageURICommand = new CopyMessageURICommand(clipboard, serviceControl);
            RetryMessageCommand = new RetryMessageCommand(eventAggregator, serviceControl);
            SearchByMessageIDCommand = new SearchByMessageIDCommand(eventAggregator, searchBar);
            ShowSagaCommand = new ShowSagaCommand(eventAggregator);
        }

        public ICommand RetryMessageCommand { get; private set; }
        public ICommand CopyConversationIDCommand { get; private set; }
        public ICommand CopyMessageURICommand { get; private set; }
        public ICommand SearchByMessageIDCommand { get; private set; }
        public ICommand ShowSagaCommand { get; private set; }

        public DiagramItemCollection DiagramItems { get; set; }

        public StoredMessage SelectedMessage { get; set; }

        private void OnSelectedMessageChanged()
        {
            if (!donotReselect && SelectedMessage != null)
            {
                eventAggregator.Publish(new SelectedMessageChanged(SelectedMessage));
            }

            donotReselect = false;
        }

        private bool donotReselect;

        public void Handle(SelectedMessageChanged message)
        {
            var storedMessage = message.Message;
            if (storedMessage == null)
                return;

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
                return;

            var messages = serviceControl.GetConversationByIdNew(conversationId).ToList();

            if (messages.Count == 0)
            {
                // SC is being silly
                LogTo.Warning("No messages found for conversation id {0}", conversationId);
                return;
            }

            CreateElements(messages);

            donotReselect = true;
            SelectedMessage = storedMessage;
        }

        private void CreateElements(List<ReceivedMessage> messages)
        {
            var modelCreator = new ModelCreator(messages);
            var endpoints = modelCreator.Endpoints;
            var handlers = modelCreator.Handlers;

            DiagramItems.Clear();
            DiagramItems.AddRange(endpoints);
            DiagramItems.AddRange(endpoints.Select(e => e.Timeline));
            DiagramItems.AddRange(handlers);
            DiagramItems.AddRange(handlers.SelectMany(h => h.Out));
        }
    }
}
