namespace ServiceInsight.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Framework;
    using Framework.Events;
    using MessageList;
    using Models;
    using Pirac;
    using ServiceControl;

    public class SagaWindowViewModel : Caliburn.Micro.Screen
    {
        SagaData data;
        StoredMessage currentMessage;
        string selectedMessageId;
        IRxEventAggregator eventAggregator;
        IWorkNotifier workNotifier;
        IServiceControl serviceControl;
        readonly MessageSelectionContext selection;

        public SagaWindowViewModel(
            IRxEventAggregator eventAggregator,
            IWorkNotifier workNotifier,
            IServiceControl serviceControl,
            IClipboard clipboard,
            MessageSelectionContext selectionContext)
        {
            this.workNotifier = workNotifier;
            this.eventAggregator = eventAggregator;
            this.serviceControl = serviceControl;
            selection = selectionContext;
            ShowSagaNotFoundWarning = false;
            CopyCommand = Command.Create(() => clipboard.CopyTo(InstallScriptText));
            ShowFlowComamnd = Command.Create(() => eventAggregator.Publish(SwitchToFlowWindow.Instance));
            eventAggregator.GetEvent<SelectedMessageChanged>().Subscribe(Handle);
        }

        public string InstallScriptText { get; set; }

        public ICommand CopyCommand { get; }

        public ICommand ShowFlowComamnd { get; }

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

        void Handle(SelectedMessageChanged @event)
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

        void UpdateInstallScriptText(StoredMessage message)
        {
            InstallScriptText = $"install-package ServiceControl.Plugin.NSB{GetMajorVersion(message)}.SagaAudit";
        }

        string GetMajorVersion(StoredMessage message)
        {
            var version = message.GetHeaderByKey(MessageHeaderKeys.Version);
            return version?.Split('.').First();
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

                    Data = serviceControl.GetSagaById(originatingSaga.SagaId);

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