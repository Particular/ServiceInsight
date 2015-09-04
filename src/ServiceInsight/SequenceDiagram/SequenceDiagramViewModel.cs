namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System.Collections.Generic;
    using System.Linq;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using global::ServiceInsight.SequenceDiagram.Drawing;
    using Models;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>
    {
        private readonly IServiceControl serviceControl;

        public SequenceDiagramViewModel(IServiceControl serviceControl)
        {
            this.serviceControl = serviceControl;
            DiagramElements = new BindableCollection<UmlViewModel>();
        }

        public BindableCollection<UmlViewModel> DiagramElements { get; set; }

        public BindableCollection<EndpointViewModel> Endpoints
        {
            get
            {
                return new BindableCollection<EndpointViewModel>(DiagramElements.OfType<EndpointViewModel>());
            }
        }

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

            CreateElements(messages);
        }

        private void CreateElements(IList<StoredMessage> messages)
        {
            var endpoints = new List<EndpointViewModel>();

            endpoints.AddRange(messages
                .Select(m => m.SendingEndpoint != null ? new EndpointViewModel(m.SendingEndpoint, m.GetHeaderByKey(MessageHeaderKeys.Version, null)) : null)
                .Distinct());

            foreach (var message in messages)
            {
                if (message.ReceivingEndpoint != null)
                {
                    var endpointViewModel = new EndpointViewModel(message.ReceivingEndpoint);

                    if (!endpoints.Contains(endpointViewModel))
                    {
                        endpoints.Add(endpointViewModel);
                    }
                }
            }

            var handlers = new List<HandlerViewModel>();

            foreach (var message in messages)
            {
                if (message.ReceivingEndpoint == null)
                {
                    continue;
                }

                var endpointViewModel = endpoints.Find(e=> IsSameEndpoint(e, message.ReceivingEndpoint, message.GetHeaderByKey(MessageHeaderKeys.Version, null)));
                var handlerViewModel = CreateHandler(endpointViewModel, message);

                handlers.Add(handlerViewModel);
            }

            var arrows = new List<UmlViewModel>();

            foreach (var message in messages)
            {
                var to = handlers.Find(h => IsSameEndpoint(h.Endpoint, message.ReceivingEndpoint, message.GetHeaderByKey(MessageHeaderKeys.Version, null)));
                var from = handlers.Find(h => IsSameEndpoint(h.Endpoint, message.SendingEndpoint, message.GetHeaderByKey(MessageHeaderKeys.Version, null)));
                var arrow = CreateArrow(message, from, to);

                arrows.Add(arrow);
            }

            DiagramElements.AddRange(endpoints);
            DiagramElements.AddRange(handlers);
            DiagramElements.AddRange(arrows);
        }

        static bool IsSameEndpoint(EndpointViewModel e1, Endpoint e2, string version = null)
        {
            if (e1 == null)
            {
                return false;
            }

            if (e2 == null)
            {
                return false;
            }

            return new EndpointViewModel(e2, version).Equals(e1);
        }

        static ArrowViewModel CreateArrow(StoredMessage message, HandlerViewModel @from, HandlerViewModel to)
        {
            var arrow = new ArrowViewModel
            {
                From = @from,
                To = to,
                Title = TypeHumanizer.ToName(message.MessageType)
            };

            if (message.MessageIntent == MessageIntent.Publish)
            {
                arrow.Type = ArrowType.Event;
            }
            else
            {
                var isTimeoutString = message.GetHeaderByKey(MessageHeaderKeys.IsSagaTimeout);
                var isTimeout = !string.IsNullOrEmpty(isTimeoutString) && bool.Parse(isTimeoutString);
                if (isTimeout)
                {
                    arrow.Type = ArrowType.Timeout;
                }
                else if (message.ReceivingEndpoint == message.SendingEndpoint)
                {
                    arrow.Type = ArrowType.Local;
                }
                else
                {
                    arrow.Type = ArrowType.Command;
                }
            }
            
            return arrow;
        }

        static HandlerViewModel CreateHandler(EndpointViewModel endpointViewModel, StoredMessage message)
        {
            var handlerViewModel = new HandlerViewModel
            {
                Endpoint = endpointViewModel,
                HandledAt = message.TimeSent
            };

            if (message.InvokedSagas != null && message.InvokedSagas.Count > 0)
            {
                handlerViewModel.PartOfSaga = TypeHumanizer.ToName(message.InvokedSagas[0].SagaType);
            }

            if (message.Status == MessageStatus.ArchivedFailure || message.Status == MessageStatus.Failed || message.Status == MessageStatus.RepeatedFailure)
            {
                handlerViewModel.State = HandlerStateType.Fail;
            }
            else
            {
                handlerViewModel.State = HandlerStateType.Success;
            }

            return handlerViewModel;
        }
    }
}
