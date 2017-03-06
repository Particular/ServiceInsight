namespace ServiceInsight.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Framework;
    using Framework.Events;
    using MessageList;
    using Models;
    using ServiceControl;
    using ServiceInsight.Framework.UI.ScreenManager;
    using ServiceInsight.MessagePayloadViewer;

    public class SagaWindowViewModel : Screen,
        IHandle<SelectedMessageChanged>,
        IHandle<ServiceControlConnectionChanged>
    {
        SagaData data;
        StoredMessage currentMessage;
        string selectedMessageId;
        IEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        IWindowManagerEx windowManager;
        readonly MessageSelectionContext selection;

        public SagaWindowViewModel(
            IEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            IServiceControl serviceControl,
            IClipboard clipboard,
            IWindowManagerEx windowManager,
            MessageSelectionContext selectionContext)
        {
            this.eventAggregator = eventAggregator;
            this.workNotifier = workNotifier;
            this.serviceControl = serviceControl;
            this.windowManager = windowManager;
            selection = selectionContext;
            ShowSagaNotFoundWarning = false;
            CopyCommand = Command.Create(arg => clipboard.CopyTo(InstallScriptText));
            ShowEntireContentCommand = Command.Create(arg => ShowEntireContent((SagaUpdatedValue)arg));
        }

        public string InstallScriptText { get; set; }

        public ICommand CopyCommand { get; }

        public ICommand ShowEntireContentCommand { get; set; }

        public void OnShowMessageDataChanged()
        {
            if (Data == null || Data.Changes == null)
            {
                return;
            }

            RefreshShowData();

            if (ShowMessageData)
            {
                RefreshMessageProperties();
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

            NotifyOfPropertyChange(() => Data);
        }

        void RefreshMessageProperties()
        {
            foreach (var message in Data.Changes.Select(c => c.InitiatingMessage).Union(Data.Changes.SelectMany(c => c.OutgoingMessages)))
            {
                message.RefreshData(serviceControl);
            }

            NotifyOfPropertyChange(() => Data);
        }

        public void Handle(SelectedMessageChanged @event)
        {
            var message = selection.SelectedMessage;
            if (message == null)
            {
                return;
            }

            UpdateInstallScriptText(message);

            RefreshSaga(message);

            SelectedMessageId = message.MessageId;
        }

        void ShowEntireContent(SagaUpdatedValue value)
        {
            windowManager.ShowDialog(new MessagePayloadViewModel(value.EffectiveValue)
            {
                DisplayName = value.SagaName
            });
        }

        public void Handle(ServiceControlConnectionChanged message)
        {
            ClearSaga();
        }

        void UpdateInstallScriptText(StoredMessage message)
        {
            InstallScriptText = $"install-package ServiceControl.Plugin.NSB{GetMajorVersion(message)}.SagaAudit";
        }

        string GetMajorVersion(StoredMessage message)
        {
            var version = message.GetHeaderByKey(MessageHeaderKeys.Version);
            return version?.Split('.').First();
        }

        void ClearSaga()
        {
            Data = null;
        }

        void RefreshSaga(StoredMessage message)
        {
            currentMessage = message;
            ShowSagaNotFoundWarning = false;

            if (message == null || message.Sagas == null || !message.Sagas.Any())
            {
                Data = null;
            }
            else
            {
                var originatingSaga = message.Sagas.First();

                if (originatingSaga != null)
                {
                    RefreshSaga(originatingSaga);
                }
            }
        }

        void RefreshSaga(SagaInfo originatingSaga)
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

                    Data = FetchOrderedSagaData(originatingSaga.SagaId);

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
                    RefreshMessageProperties();
                }
            }
        }

        private SagaData FetchOrderedSagaData(Guid sagaId)
        {
            var sagaData = serviceControl.GetSagaById(sagaId);
            if (sagaData?.Changes != null)
            {
                sagaData.Changes = sagaData.Changes.OrderBy(x => x.StartTime)
                                                   .ThenBy(x => x.FinishTime)
                                                   .ToList();

                foreach (var timeout in sagaData.Changes.SelectMany(update => update.TimeoutMessages))
                {
                    timeout.HasBeenProcessed =
                        sagaData.Changes.Any(update => update.InitiatingMessage.MessageId == timeout.MessageId);
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
            get { return data; }

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

        public void RefreshSaga()
        {
            RefreshSaga(currentMessage);
        }

        public string SelectedMessageId
        {
            get { return selectedMessageId; }

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
    }
}