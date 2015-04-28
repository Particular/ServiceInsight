namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Framework;
    using Models;
    using Particular.ServiceInsight.Desktop.Explorer.EndpointExplorer;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Framework.UI.ScreenManager;
    using Particular.ServiceInsight.Desktop.MessageFlow;
    using Particular.ServiceInsight.Desktop.MessageList;
    using ReactiveUI;
    using Search;
    using ServiceControl;

    public class SequenceDiagramViewModel : Screen, IHandle<SelectedMessageChanged>, IHandle<MessageInfo.HiliteEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IWindowManagerEx windowManager;
        private readonly IServiceControl serviceControl;
        private readonly Func<ExceptionDetailViewModel> exceptionDetailViewModel;

        private bool donotReselect;

        public SequenceDiagramViewModel(
            IClipboard clipboard,
            IEventAggregator eventAggregator,
            IWindowManagerEx windowManager,
            IServiceControl serviceControl,
            Func<ExceptionDetailViewModel> exceptionDetailViewModel,
            SearchBarViewModel searchBar,
            EndpointExplorerViewModel endpointExplorer,
            MessageListViewModel messageList)
        {
            this.windowManager = windowManager;
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
            this.exceptionDetailViewModel = exceptionDetailViewModel;

            CopyConversationIDCommand = new CopyConversationIDCommand(clipboard);
            CopyMessageURICommand = new CopyMessageURICommand(clipboard, serviceControl);
            RetryMessageCommand = new RetryMessageCommand(eventAggregator, serviceControl);
            SearchByMessageIDCommand = new SearchByMessageIDCommand(eventAggregator, searchBar);
            ShowSagaCommand = new ShowSagaCommand(eventAggregator, endpointExplorer, messageList);
        }

        public ReactiveList<EndpointInfo> Endpoints { get; set; }

        public IEnumerable<MessageInfo> Messages { get; set; }
        public MessageInfo SelectedMessage { get; set; }

        public ICommand RetryMessageCommand { get; private set; }
        public ICommand CopyConversationIDCommand { get; private set; }
        public ICommand CopyMessageURICommand { get; private set; }
        public ICommand SearchByMessageIDCommand { get; private set; }
        public ICommand ShowSagaCommand { get; private set; }

        private void OnSelectedMessageChanged()
        {
            if (!donotReselect && SelectedMessage != null)
                eventAggregator.Publish(new SelectedMessageChanged(SelectedMessage.Message));

            donotReselect = false;
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

            CreateEndpoints(messages);

            CreateMessages(messages);

            donotReselect = true;

            SelectedMessage = Messages.FirstOrDefault(m =>
                m.Message.MessageId == message.Message.MessageId &&
                m.Message.TimeSent == message.Message.TimeSent &&
                m.Message.Id == message.Message.Id);
        }

        public void Handle(MessageInfo.HiliteEvent hiliteEvent)
        {
            var hiliteOn = false;

            foreach (var messageInfo in Messages)
            {
                if (!hiliteEvent.Hilite)
                {
                    messageInfo.VerticalHilite = false;
                    continue;
                }

                if (messageInfo.Message.Id == hiliteEvent.TriggerMessageId)
                    hiliteOn = false;

                if (hiliteOn)
                {
                    messageInfo.VerticalHilite = true;
                    messageInfo.VerticalHiliteIndex = hiliteEvent.VerticalHiliteIndex;
                    messageInfo.VerticalHiliteIsPublished = hiliteEvent.IsPublished;
                }

                if (hiliteEvent.RelatedToMessageId == messageInfo.Message.MessageId &&
                    messageInfo.Message.ReceivingEndpoint != null &&
                    messageInfo.Message.ReceivingEndpoint.Name == hiliteEvent.SendingEndpointName)
                    hiliteOn = true;
            }
        }

        private void CreateEndpoints(IEnumerable<StoredMessage> messages)
        {
            Endpoints = new ReactiveList<EndpointInfo>(messages
                .OrderBy(m => m.TimeSent)
                .SelectMany(m => new[] {
                    m.SendingEndpoint != null ? new EndpointInfo(m.SendingEndpoint, m) : null,
                    m.ReceivingEndpoint != null ? new EndpointInfo(m.ReceivingEndpoint, m) : null
                })
                .Where(e => e != null) // TODO report these as they shouldn't happen
                .Distinct());
        }

        private void CreateMessages(IEnumerable<StoredMessage> messages)
        {
            var orderedMessages = messages.OrderBy(m => m.TimeSent);

            Messages = orderedMessages
                .Select(m => new MessageInfo(eventAggregator, windowManager, exceptionDetailViewModel, this, m, Endpoints))
                .ToList();

            Messages.First().IsFirst = true;

            var lookup = Endpoints.ToDictionary(e => e.FullName);
            foreach (var message in Messages)
            {
                eventAggregator.Subscribe(message);

                if (message.Message.SendingEndpoint == null)
                    continue;

                var hiliteOn = false;
                EndpointInfo sendingEndpoint;

                if (!lookup.TryGetValue(message.Message.SendingEndpoint.Name, out sendingEndpoint))
                    continue;

                foreach (var tempMessage in Messages)
                {
                    if (message.Message.Id == tempMessage.Message.Id)
                        break;

                    if (hiliteOn)
                    {
                        tempMessage.SetMessageLine(sendingEndpoint, message);
                    }

                    if (message.Message.RelatedToMessageId == tempMessage.Message.MessageId &&
                        tempMessage.Message.ReceivingEndpoint != null &&
                        tempMessage.Message.ReceivingEndpoint.Name == message.Message.SendingEndpoint.Name)
                        hiliteOn = true;
                }
            }
        }
    }
}