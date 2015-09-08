namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Collections.Generic;
    using System.Linq;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using global::ServiceInsight.SequenceDiagram;
    using global::ServiceInsight.SequenceDiagram.Drawing;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IServiceControl serviceControl;

        public SequenceDiagramViewModel(IServiceControl serviceControl)
        {
            this.serviceControl = serviceControl;
            Endpoints = new BindableCollection<EndpointViewModel>();
        }

        public BindableCollection<EndpointViewModel> Endpoints { get; set; }

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
        }

        private void CreateElements(List<ReceivedMessage> messages)
        {
            var endpoints = new ModelCreator(messages).GetModel();

            Endpoints.AddRange(endpoints);
        }
    }
}
