using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.Models;
using NServiceBus.Profiler.Desktop.ServiceControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaWindowViewModel : Screen, ISagaWindowViewModel, IHandle<SelectedMessageChanged>
    {
        private IEventAggregator _eventAggregator;
        private IServiceControl _serviceControl;

        public SagaWindowViewModel(IEventAggregator eventAggregator, IServiceControl serviceControl)
        {
            _eventAggregator = eventAggregator;
            _serviceControl = serviceControl;
            ShowSagaNotFoundWarning = false;
        }

        public void OnShowMessageDataChanged()
        {
            RefreshShowData();

            if (ShowMessageData)
            {
                RefreshMessageProperties();
            }
        }

        private void RefreshShowData()
        {
            if (Data == null || Data.Changes == null) return;

            var messages = Data.Changes
                               .Select(c => c.InitiatingMessage)
                               .Union(Data.Changes.SelectMany(c => c.OutgoingMessages));

            foreach (var message in messages)
            {
                message.ShowData = ShowMessageData;
            }

            NotifyOfPropertyChange(() => Data);
        }

        private void RefreshMessageProperties()
        {
            RefreshMessageProperties(Data.Changes.Select(c => c.InitiatingMessage).Union(Data.Changes.SelectMany(c => c.OutgoingMessages)));
        }

        private async void RefreshMessageProperties(IEnumerable<SagaMessage> messages)
        {
            foreach (var message in messages)
            {
                await message.RefreshData(_serviceControl);
            }

            NotifyOfPropertyChange(() => Data);
        }

        public async void Handle(SelectedMessageChanged @event)
        {
            var message = @event.Message;
            await RefreshSaga(message, a => true);
        }

        private async Task RefreshSaga(StoredMessage message, Func<string, bool> HasChanged)
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

                if (Data == null || Data.SagaId != originatingSaga.SagaId)
                {
                    if (originatingSaga != null)
                    {
                        if (HasChanged(originatingSaga.SagaId.ToString()))
                        {
                            await RefreshSaga(originatingSaga);
                        }
                    }
                }
            }
        }

        private async Task RefreshSaga(SagaInfo originatingSaga)
        {
            _eventAggregator.Publish(new WorkStarted("Loading message body..."));

            if (Data == null || Data.SagaId != originatingSaga.SagaId)
            {
                Data = await _serviceControl.GetSagaById(originatingSaga.SagaId.ToString());

                if (Data == SagaData.Empty)
                {
                    ShowSagaNotFoundWarning = true;
                    Data = null;
                }
                else if (Data != null && Data.Changes != null)
                {
                    ProcessDataValues(Data.Changes);
                }
                else
                {
                    Data = null;
                }
            }

            if (ShowMessageData)
            {
                RefreshMessageProperties();
            }

            RefreshShowData();

            _eventAggregator.Publish(new WorkFinished());
        }

        private static void ProcessDataValues(IEnumerable<SagaUpdate> list)
        {
            IList<SagaUpdatedValue> oldValues = new List<SagaUpdatedValue>();
            foreach (var change in list)
            {
                ProcessChange(oldValues, change.Values);
                oldValues = change.Values;
            }
        }

        private static void ProcessChange(IList<SagaUpdatedValue> oldValues, IList<SagaUpdatedValue> newValues)
        {
            foreach (var value in newValues)
            {
                var oldValue = oldValues.FirstOrDefault(v => v.Name == value.Name);
                value.OldValue = oldValue != null ? oldValue.NewValue : string.Empty;
            }
        }

        private StoredMessage currentMessage;

        public bool ShowSagaNotFoundWarning { get; set; }

        public bool HasSaga { get { return Data != null; } }
        public SagaData Data { get; private set; }
        public bool ShowEndpoints { get; set; }
        public bool ShowMessageData { get; set; }

        public void ShowFlow()
        {
            _eventAggregator.Publish(new SwitchToFlowWindow());
        }


        public async Task RefreshSaga()
        {
            await RefreshSaga(currentMessage, _serviceControl.HasSagaChanged);
        }
    }

    public interface ISagaWindowViewModel : IScreen
    {
        bool ShowSagaNotFoundWarning { get; }
        bool ShowMessageData { get; }
        bool ShowEndpoints { get; set; }
        bool HasSaga { get; }
        SagaData Data { get; }
        void ShowFlow();

        Task RefreshSaga();
    }
}
