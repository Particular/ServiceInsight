namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Collections.Generic;
    using System.Linq;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using global::ServiceInsight.SequenceDiagram.Drawing;
    using Models;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using ReactiveUI;
    using ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IServiceControl serviceControl;
        public ReactiveList<UmlViewModel> DiagramElements
        {
            get;
            set;
        }

        public SequenceDiagramViewModel(IServiceControl serviceControl)
        {
            this.serviceControl = serviceControl;
        }

        //public ReactiveList<EndpointInfo> Endpoints { get; set; }

        public void Handle(SelectedMessageChanged message)
        {
            var storedMessage = message.Message;
            if (storedMessage == null)
                return;

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
                return;

            var messages = serviceControl.GetConversationById(conversationId).ToList();

            if (messages.Count == 0)
            {
                // SC is being silly
                LogTo.Warning("No messages found for conversation id {0}", conversationId);
                return;
            }

            CreateEndpoints(messages);
        }

        private void CreateEndpoints(IList<StoredMessage> messages)
        {
            var endpointInfos = messages
                .OrderBy(m => m.TimeSent)
                .Select(m => m.SendingEndpoint != null ? new EndpointViewModel(m.SendingEndpoint, m.GetHeaderByKey(MessageHeaderKeys.Version)) : null)
                .Where(e => e != null) // TODO report these as they shouldn't happen
                .Distinct()
                .ToList();

            foreach (var message in messages)
            {
                if (message.ReceivingEndpoint == null || endpointInfos.Exists(e => e.FullName == message.ReceivingEndpoint.Name && e.Host == message.ReceivingEndpoint.Host))
                {
                    continue;
                }

                endpointInfos.Add(new EndpointViewModel(message.ReceivingEndpoint, "Not Available"));
            }

            DiagramElements = new ReactiveList<UmlViewModel>(endpointInfos);
        }
    }
}