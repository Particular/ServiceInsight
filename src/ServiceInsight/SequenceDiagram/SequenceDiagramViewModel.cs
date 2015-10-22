namespace ServiceInsight.SequenceDiagram
{
    using System.Collections.Generic;
    using System.Linq;
    using Anotar.Serilog;
    using Autofac;
    using Caliburn.Micro;
    using Diagram;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IServiceControl serviceControl;
        readonly IEventAggregator eventAggregator;
        readonly IContainer container;

        public SequenceDiagramViewModel(IServiceControl serviceControl, IEventAggregator eventAggregator, IContainer container)
        {
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            this.container = container;

            DiagramItems = new DiagramItemCollection();
        }

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
            var modelCreator = new ModelCreator(messages, container);
            var endpoints = modelCreator.Endpoints;
            var handlers = modelCreator.Handlers;
            var routes = modelCreator.Routes;

            DiagramItems.Clear();
            DiagramItems.AddRange(endpoints);
            DiagramItems.AddRange(endpoints.Select(e => e.Timeline));
            DiagramItems.AddRange(handlers);
            DiagramItems.AddRange(handlers.SelectMany(h => h.Out));
            DiagramItems.AddRange(routes);
        }
    }
}
