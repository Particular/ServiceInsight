using ServiceInsight.Explorer;
using ServiceInsight.ExtensionMethods;

namespace ServiceInsight.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Anotar.Serilog;
    using Caliburn.Micro;
    using Diagram;
    using DiagramLegend;
    using Framework;
    using Framework.Commands;
    using Framework.Events;
    using Framework.Settings;
    using MessageList;
    using Models;
    using ServiceControl;
    using Settings;

    public class SequenceDiagramViewModel : Screen,
        IHandleWithTask<SelectedMessageChanged>,
        IHandle<ScrollDiagramItemIntoView>,
        IHandle<SelectedExplorerItemChanged>,
        IMessageCommandContainer
    {
        readonly ServiceControlClientRegistry clientRegistry;
        readonly ISettingsProvider settingsProvider;
        readonly SequenceDiagramSettings settings;
        string loadedConversationId;
        SequenceDiagramView view;
        ExplorerItem selectedExplorerItem;

        const string SequenceDiagramDocumentationUrl = "http://docs.particular.net/serviceinsight/no-data-available";
        const string SequenceDiagramLegendUrl = "https://docs.particular.net/serviceinsight/sequence-diagram/#what-is-on-the-diagram";

        public SequenceDiagramViewModel(
            ISettingsProvider settingsProvider,
            MessageSelectionContext selectionContext,
            DiagramLegendViewModel diagramLegend,
            CopyConversationIDCommand copyConversationIDCommand,
            CopyMessageURICommand copyMessageURICommand,
            RetryMessageCommand retryMessageCommand,
            SearchByMessageIDCommand searchByMessageIDCommand,
            ChangeSelectedMessageCommand changeSelectedMessageCommand,
            ShowExceptionCommand showExceptionCommand,
            ReportMessageCommand reportMessageCommand,
            ServiceControlClientRegistry clientRegistry)
        {
            this.settingsProvider = settingsProvider;
            this.clientRegistry = clientRegistry;

            Selection = selectionContext;
            CopyConversationIDCommand = copyConversationIDCommand;
            CopyMessageURICommand = copyMessageURICommand;
            RetryMessageCommand = retryMessageCommand;
            SearchByMessageIDCommand = searchByMessageIDCommand;
            ChangeSelectedMessageCommand = changeSelectedMessageCommand;
            ShowExceptionCommand = showExceptionCommand;
            ReportMessageCommand = reportMessageCommand;
            OpenLink = Command.Create(arg => new NetworkOperations().Browse(SequenceDiagramDocumentationUrl));
            OpenLegendUrl = Command.Create(arg => new NetworkOperations().Browse(SequenceDiagramLegendUrl));
            DiagramLegend = diagramLegend;
            DiagramItems = new DiagramItemCollection();
            HeaderItems = new DiagramItemCollection();

            settings = settingsProvider.GetSettings<SequenceDiagramSettings>();
            ShowLegend = settings.ShowLegend;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.view = (SequenceDiagramView)view;
        }
        
        private IServiceControl ServiceControl => selectedExplorerItem.GetServiceControlClient(clientRegistry);

        public ICommand OpenLink { get; }

        public ICommand OpenLegendUrl { get; }

        public ICommand CopyConversationIDCommand { get; }

        public ICommand CopyMessageURICommand { get; }

        public ICommand RetryMessageCommand { get; }

        public ICommand SearchByMessageIDCommand { get; }

        public ICommand ChangeSelectedMessageCommand { get; }

        public ICommand ShowExceptionCommand { get; }

        public ICommand ReportMessageCommand { get; }

        public DiagramLegendViewModel DiagramLegend { get; }

        public DiagramItemCollection DiagramItems { get; }

        public string ErrorMessage { get; set; }

        public ReportMessageCommand.ReportMessagePackage ReportPackage { get; set; }

        public bool ShowLegend { get; set; }

        public void OnShowLegendChanged()
        {
            settings.ShowLegend = ShowLegend;
            settingsProvider.SaveSettings(settings);
        }

        public bool HasItems => DiagramItems != null && DiagramItems.Count > 0;

        public DiagramItemCollection HeaderItems { get; }

        public MessageSelectionContext Selection { get; }

        protected override void OnActivate()
        {
            base.OnActivate();
            DiagramLegend.ActivateWith(this);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            DiagramLegend.DeactivateWith(this);
        }

        public async Task Handle(SelectedMessageChanged message)
        {
            try
            {
                var conversationId = Selection?.SelectedMessage?.ConversationId;
                if (string.IsNullOrEmpty(conversationId))
                {
                    ClearState();
                    return;
                }

                // If we've already displayed this diagram
                if (loadedConversationId == conversationId && DiagramItems.Any()) 
                {
                    RefreshSelection();
                    return;
                }

                var messages = default(List<StoredMessage>);
                
                if (ServiceControl != null)
                {
                    messages = (await ServiceControl.GetConversationById(conversationId)).ToList();
                }
                
                if (messages?.Count == 0)
                {
                        
                    LogTo.Warning("No messages found for conversation id {0}", conversationId);
                    ClearState();
                    return;
                }

                CreateElements(messages);
                loadedConversationId = conversationId;
                RefreshSelection();
            }
            catch (Exception ex)
            {
                ClearState();
                ErrorMessage = $"There was an error processing the message data.";
                ReportPackage = new ReportMessageCommand.ReportMessagePackage(ex, Selection?.SelectedMessage);
            }
        }

        void RefreshSelection()
        {
            foreach (var item in DiagramItems.OfType<Handler>())
            {
                item.IsFocused = false;
            }

            foreach (var item in DiagramItems.OfType<Arrow>())
            {
                if (string.Equals(item.SelectedMessage.Id, Selection.SelectedMessage.Id, StringComparison.InvariantCultureIgnoreCase))
                {
                    item.IsFocused = true;
                    continue;
                }

                item.IsFocused = false;
            }
        }

        void CreateElements(List<StoredMessage> messages)
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

            NotifyOfPropertyChange(nameof(HasItems));
        }

        void ClearState()
        {
            ErrorMessage = "";
            DiagramItems.Clear();
            HeaderItems.Clear();
            NotifyOfPropertyChange(nameof(HasItems));
        }

        public void Handle(ScrollDiagramItemIntoView @event)
        {
            var diagramItem = DiagramItems.OfType<Arrow>()
                .FirstOrDefault(a => a.SelectedMessage.Id == @event.Message.Id);

            if (diagramItem != null)
            {
                view?.diagram.BringIntoView(diagramItem);
            }
        }
        
        public void Handle(SelectedExplorerItemChanged @event)
        {
            selectedExplorerItem = @event.SelectedExplorerItem;
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