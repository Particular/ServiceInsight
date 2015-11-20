namespace ServiceInsight.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Diagram;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
    using Particular.ServiceInsight.Desktop.Framework.Events;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;

    public class SequenceDiagramViewModel : Screen,
        IHandle<SelectedMessageChanged>,
        IMessageCommandContainer
    {
        private readonly IServiceControl serviceControl;
        readonly IEventAggregator eventAggregator;
        string loadedConversationId;

        public SequenceDiagramViewModel(
            IServiceControl serviceControl,
            IEventAggregator eventAggregator,
            CopyConversationIDCommand copyConversationIDCommand,
            CopyMessageURICommand copyMessageURICommand,
            RetryMessageCommand retryMessageCommand,
            SearchByMessageIDCommand searchByMessageIDCommand,
            ChangeSelectedMessageCommand changeSelectedMessageCommand,
            ShowExceptionCommand showExceptionCommand)
        {
            this.serviceControl = serviceControl;
            this.eventAggregator = eventAggregator;
            this.CopyConversationIDCommand = copyConversationIDCommand;
            this.CopyMessageURICommand = copyMessageURICommand;
            this.RetryMessageCommand = retryMessageCommand;
            this.SearchByMessageIDCommand = searchByMessageIDCommand;
            this.ChangeSelectedMessageCommand = changeSelectedMessageCommand;
            this.ShowExceptionCommand = showExceptionCommand;

            DiagramItems = new DiagramItemCollection();
            HeaderItems = new DiagramItemCollection();
        }

        public ICommand CopyConversationIDCommand { get; private set; }

        public ICommand CopyMessageURICommand { get; private set; }

        public ICommand RetryMessageCommand { get; private set; }

        public ICommand SearchByMessageIDCommand { get; private set; }

        public ICommand ChangeSelectedMessageCommand { get; private set; }

        public ICommand ShowExceptionCommand { get; private set; }

        public DiagramItemCollection DiagramItems { get; set; }

        public DiagramItemCollection HeaderItems { get; set; }

        public StoredMessage SelectedMessage { get; set; }

        private void OnSelectedMessageChanged()
        {
            if (SelectedMessage != null)
            {
                eventAggregator.Publish(new SelectedMessageChanged(SelectedMessage));
            }
        }

        public void Handle(SelectedMessageChanged message)
        {
            var storedMessage = message.Message;
            if (storedMessage == null)
            {
                ClearState();
                return;
            }

            var conversationId = storedMessage.ConversationId;
            if (conversationId == null)
            {
                ClearState();
                return;
            }

            if (loadedConversationId == conversationId && DiagramItems.Any()) //If we've already displayed this diagram
            {
                RefreshSelection(storedMessage.Id);
                return;
            }

            var messages = serviceControl.GetConversationByIdNew(conversationId).ToList();
            if (messages.Count == 0)
            {
                // SC is being silly
                LogTo.Warning("No messages found for conversation id {0}", conversationId);
                return;
            }

            CreateElements(messages);
            loadedConversationId = conversationId;
            SelectedMessage = storedMessage;
        }

        void RefreshSelection(string selectedId)
        {
            foreach (var item in DiagramItems.OfType<Handler>())
            {
                item.IsFocused = false;
            }

            foreach (var item in DiagramItems.OfType<Arrow>())
            {
                if (string.Equals(item.SelectedMessage.Id, selectedId, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.IsFocused = true;
                    SelectedMessage = item.SelectedMessage;
                    continue;
                }

                item.IsFocused = false;
            }
        }

        void CreateElements(List<ReceivedMessage> messages)
        {
            var modelCreator = new ModelCreator(messages, this);
            var endpoints = modelCreator.Endpoints;
            var handlers = modelCreator.Handlers;
            var routes = modelCreator.Routes;

            ClearState();

            DiagramItems.AddRange(endpoints);
            DiagramItems.AddRange(endpoints.Select(e => e.Timeline));
            DiagramItems.AddRange(handlers);
            DiagramItems.AddRange(handlers.SelectMany(h => h.Out));
            DiagramItems.AddRange(routes);

            HeaderItems.AddRange(endpoints);
        }

        void ClearState()
        {
            DiagramItems.Clear();
            HeaderItems.Clear();
        }
    }

    public interface IMessageCommandContainer
    {
        ICommand CopyConversationIDCommand { get; }
        ICommand CopyMessageURICommand { get; }
        ICommand RetryMessageCommand { get; }
        ICommand SearchByMessageIDCommand { get; }
        ICommand ChangeSelectedMessageCommand { get; }
        ICommand ShowExceptionCommand { get; }
    }
}