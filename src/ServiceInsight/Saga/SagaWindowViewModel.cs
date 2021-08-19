﻿namespace ServiceInsight.Saga
{
    using System;
    using Explorer;
    using ExtensionMethods;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Framework;
    using Framework.Events;
    using MessageList;
    using Models;
    using ServiceControl;
    using Framework.UI.ScreenManager;
    using MessagePayloadViewer;

    public class SagaWindowViewModel : Screen,
        IHandleWithTask<SelectedMessageChanged>,
        IHandle<SelectedExplorerItemChanged>,
        IHandle<ServiceControlDisconnected>
    {
        readonly IEventAggregator eventAggregator;
        readonly IWorkNotifier workNotifier;
        readonly IServiceInsightWindowManager windowManager;
        readonly MessageSelectionContext selection;
        readonly ServiceControlClientRegistry clientRegistry;

        SagaData data;
        StoredMessage currentMessage;
        string selectedMessageId;
        ExplorerItem selectedExplorerItem;

        public SagaWindowViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            IClipboard clipboard,
            IServiceInsightWindowManager windowManager,
            MessageSelectionContext selectionContext,
            ServiceControlClientRegistry clientRegistry)
        {
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.windowManager = windowManager;
            selection = selectionContext;
            this.clientRegistry = clientRegistry;
            ShowSagaNotFoundWarning = false;
            CopyCommand = Command.Create(arg => clipboard.CopyTo(InstallScriptText));
            ShowEntireContentCommand = Command.Create(arg => ShowEntireContent((SagaUpdatedValue)arg));
        }

        IServiceControl ServiceControl => selectedExplorerItem.GetServiceControlClient(clientRegistry);

        public string InstallScriptText { get; } = "install-package NServiceBus.SagaAudit";

        public ICommand CopyCommand { get; }

        public ICommand ShowEntireContentCommand { get; set; }

        public async void OnShowMessageDataChanged()
        {
            if (Data?.Changes == null)
            {
                return;
            }

            RefreshShowData();

            if (ShowMessageData)
            {
                await RefreshMessageProperties();
            }
        }

        void RefreshShowData()
        {
            var messages = Data.Changes.Where(c => !c.MissingData)
                               .Select(c => c.InitiatingMessage)
                               .Union(Data.Changes.Where(c => !c.MissingData).SelectMany(c => c.OutgoingMessages));

            foreach (var message in messages)
            {
                message.ShowData = ShowMessageData;
            }

            NotifyOfPropertyChange(nameof(Data));
        }

        async Task RefreshMessageProperties()
        {
            if (ServiceControl != null)
            {
                var messages = Data.Changes.Select(c => c.InitiatingMessage)
                    .Union(Data.Changes.SelectMany(c => c.OutgoingMessages))
                    .ToList();
                if (messages.All(x => string.IsNullOrEmpty(x.BodyUrl)))
                {
                    var auditMessages = await ServiceControl.GetAuditMessages(searchQuery: Data.SagaId.ToString())
                        .ConfigureAwait(false);
                    messages.ForEach(a =>
                        a.BodyUrl = auditMessages.Result.FirstOrDefault(x => x.MessageId == a.MessageId)?.BodyUrl);
                }

                foreach (var message in messages)
                {
                    await message.RefreshData(ServiceControl);
                }

                NotifyOfPropertyChange(nameof(Data));
            }
        }

        public async Task Handle(SelectedMessageChanged @event)
        {
            var message = selection.SelectedMessage;
            if (message == null)
            {
                ClearSaga();
                return;
            }

            await RefreshSaga(message);

            SelectedMessageId = message.MessageId;
        }

        void ShowEntireContent(SagaUpdatedValue value)
        {
            windowManager.ShowDialog(new MessagePayloadViewModel(value.EffectiveValue)
            {
                DisplayName = value.SagaName
            });
        }

        public void Handle(SelectedExplorerItemChanged @event)
        {
            selectedExplorerItem = @event.SelectedExplorerItem;
        }

        void ClearSaga()
        {
            Data = null;
        }

        async Task RefreshSaga(StoredMessage message)
        {
            currentMessage = message;
            ShowSagaNotFoundWarning = false;

            if (message?.Sagas == null || !message.Sagas.Any())
            {
                Data = null;
            }
            else
            {
                var originatingSaga = message.Sagas.First();

                if (originatingSaga != null)
                {
                    await RefreshSaga(originatingSaga);
                }
            }
        }

        async Task RefreshSaga(SagaInfo originatingSaga)
        {
            using (workNotifier.NotifyOfWork("Loading message body..."))
            {
                var previousSagaId = Guid.Empty;

                if (Data == null || Data.SagaId != originatingSaga.SagaId)
                {
                    if (Data != null)
                    {
                        previousSagaId = Data.SagaId;
                    }

                    Data = await FetchOrderedSagaData(originatingSaga.SagaId);

                    if (Data != null)
                    {
                        if (Data.SagaId == Guid.Empty)
                        {
                            ShowSagaNotFoundWarning = true;
                            Data = null;
                        }
                        else if (Data.Changes != null)
                        {
                            ProcessDataValues(Data.Changes);
                        }
                        else
                        {
                            Data = null;
                        }
                    }
                }

                if (Data == null || Data.Changes == null)
                {
                    return;
                }

                // Skip refreshing if we already displaying the correct saga data
                if (previousSagaId == Data.SagaId)
                {
                    return;
                }

                RefreshShowData();

                if (ShowMessageData)
                {
                    await RefreshMessageProperties();
                }
            }
        }

        async Task<SagaData> FetchOrderedSagaData(Guid sagaId)
        {
            var sagaData = default(SagaData);

            if (ServiceControl != null)
            {
                sagaData = await ServiceControl.GetSagaById(sagaId);
            }

            if (sagaData?.Changes != null)
            {
                sagaData.Changes = sagaData.Changes.OrderBy(x => x.StartTime)
                                                   .ThenBy(x => x.FinishTime)
                                                   .ToList();

                foreach (var timeout in sagaData.Changes.SelectMany(update => update.TimeoutMessages))
                {
                    timeout.HasBeenProcessed =
                        sagaData.Changes.Any(update => update.InitiatingMessage?.MessageId == timeout.MessageId);
                }
            }

            return sagaData;
        }

        static void ProcessDataValues(IEnumerable<SagaUpdate> list)
        {
            var oldValues = new List<SagaUpdatedValue>();
            foreach (var change in list)
            {
                ProcessChange(oldValues, change.Values);
                oldValues = change.Values;
            }
        }

        static void ProcessChange(IList<SagaUpdatedValue> oldValues, IEnumerable<SagaUpdatedValue> newValues)
        {
            foreach (var value in newValues)
            {
                var oldValueViewModel = oldValues.FirstOrDefault(v => v.Name == value.Name);
                value.UpdateOldValue(oldValueViewModel);
            }
        }

        public bool ShowSagaNotFoundWarning { get; set; }

        public bool HasSaga => Data != null;

        public SagaData Data
        {
            get => data;

            private set
            {
                data = value;
                Header = new SagaHeader(data);
                Footer = new SagaFooter(data);
            }
        }

        public SagaHeader Header { get; private set; }

        public SagaFooter Footer { get; private set; }

        public bool ShowEndpoints { get; set; }

        public bool ShowMessageData { get; set; }

        public void ShowFlow()
        {
            eventAggregator.PublishOnUIThread(SwitchToFlowWindow.Instance);
        }

        public Task RefreshSaga()
        {
            return RefreshSaga(currentMessage);
        }

        public string SelectedMessageId
        {
            get => selectedMessageId;

            set
            {
                selectedMessageId = value;
                UpdateSelectedMessages();
            }
        }

        void UpdateSelectedMessages()
        {
            if (Data == null)
            {
                return;
            }

            foreach (var step in Data.Changes.Where(c => !c.MissingData))
            {
                SetSelected(step.InitiatingMessage, selectedMessageId);
                SetSelected(step.OutgoingMessages, selectedMessageId);
            }
        }

        void SetSelected(IEnumerable<SagaMessage> messages, string id)
        {
            if (messages == null)
            {
                return;
            }

            foreach (var message in messages)
            {
                SetSelected(message, id);
            }
        }

        void SetSelected(SagaMessage message, string id)
        {
            message.IsSelected = message.MessageId == id;
        }

        public void Handle(ServiceControlDisconnected message)
        {
            if (selectedExplorerItem == null || selectedExplorerItem == message.ExplorerItem)
            {
                selectedExplorerItem = null;
                ClearSaga();
            }
        }
    }
}
