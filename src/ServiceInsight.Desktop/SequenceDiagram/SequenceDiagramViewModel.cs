using System.Collections.Generic;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Linq;
    using Caliburn.Micro;
    using Events;
    using ReactiveUI;
    using ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IServiceControl serviceControl;

        public SequenceDiagramViewModel(IServiceControl serviceControl)
        {
            this.serviceControl = serviceControl;
        }

        //public SequenceDiagramViewModel()
        //{
        //    Endpoints = new[]
        //    {
        //        new EndpointInfo { Name = "ECommerce", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
        //        new EndpointInfo { Name = "Sales", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
        //        new EndpointInfo { Name = "CustomerRelations", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
        //        new EndpointInfo { Name = "ContentManagement", Version = "4.6.1", Host = "@machinename", Active = "Yes" },
        //        new EndpointInfo { Name = "Operations", Version = "4.6.1", Host = "@machinename", Active = "Yes" }
        //    };

        //    Messages = new[]
        //    {
        //        new MessageInfo { Endpoints = Endpoints, Name = "SubmitOrder", SagaName = "ProcessOrderSaga", IsStartMessage = true, FromEndpoint = "ECommerce", ToEndpoints = new[] { "Sales" } },
        //        new MessageInfo { Endpoints = Endpoints, Name = "OrderPlaced", IsEvent = true, FromEndpoint = "Sales", ToEndpoints = new[] { "ECommerce" }, TimeoutEndpoints = new[] { "Sales" } },
        //        new MessageInfo { Endpoints = Endpoints, Name = "OrderAccepted", SagaName = "ProcessOrderSaga", IsEvent = true, FromEndpoint = "Sales", ToEndpoints = new[] { "CustomerRelations", "ContentManagement" }, },
        //        new MessageInfo { Endpoints = Endpoints, Name = "ProvisionDownloadRequest", FromEndpoint = "ContentManagement", ToEndpoints = new[] { "Operations" }, },
        //        new MessageInfo { Endpoints = Endpoints, Name = "ProvisionDownloadResponse", FromEndpoint = "Operations", ToEndpoints = new[] { "ContentManagement" }, },
        //        new MessageInfo { Endpoints = Endpoints, Name = "DownloadIsReady", IsEvent = true, FromEndpoint = "ContentManagement", ToEndpoints = new[] { "ECommerce" }, }
        //    };
        //}

        public ReactiveList<EndpointInfo> Endpoints { get; set; }

        public IEnumerable<MessageInfo> Messages { get; set; }

        public void Handle(SelectedMessageChanged message)
        {
            var storedMessage = message.Message;
            if (storedMessage == null)
                return;

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
                return;

            var messages = serviceControl.GetConversationById(conversationId).ToList();

            CreateEndpoints(messages);

            CreateMessages(messages);
        }

        private void CreateEndpoints(IEnumerable<StoredMessage> messages)
        {
            Endpoints = new ReactiveList<EndpointInfo>(messages
                .OrderBy(m => m.TimeSent)
                .SelectMany(m => new[] { m.SendingEndpoint, m.ReceivingEndpoint })
                .Select(e => new EndpointInfo(e))
                .Distinct());
        }

        private void CreateMessages(IEnumerable<StoredMessage> messages)
        {
            Messages = messages.OrderBy(m => m.TimeSent).Select(m => new MessageInfo(m, Endpoints)).ToList();

            Messages.First().IsFirst = true;
        }
    }
}