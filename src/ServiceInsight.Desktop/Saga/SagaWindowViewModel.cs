using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using NServiceBus.Profiler.Desktop.ServiceControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaWindowViewModel : Screen, ISagaWindowViewModel, IHandle<SelectedMessageChanged>
    {
        private ISagaWindowView _view;
        private IEventAggregator _eventAggregator;
        private IServiceControl _serviceControl;

        public SagaWindowViewModel(IEventAggregator eventAggregator,
            IServiceControl serviceControl)
        {
            _eventAggregator = eventAggregator;
            _serviceControl = serviceControl;

            PropertyChanged += SagaWindowViewModel_PropertyChanged;
        }

        void SagaWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowMessageData")
            {
                if (this.ShowMessageData)
                {
                    RefreshMessageProperties();
                }
            }
        }

        private void RefreshShowData()
        {
            if (Data != null)
            {
                foreach (var message in Data.Changes.Select(c => c.InitiatingMessage).Union(Data.Changes.SelectMany(c => c.OutgoingMessages))) { message.ShowData = showMessageData; };
            }
            NotifyOfPropertyChange("Data");
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
            NotifyOfPropertyChange("Data");
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (ISagaWindowView)view;

            //CreateMockSaga();
        }

        private void CreateMockSaga()
        {
            var sagaDataText = System.IO.File.ReadAllText("saga\\saga.data").Replace("\r", "").Replace("\n", "");
            Data = new SagaData 
                    { 
                        SagaType = "ProcessOrderSaga",
                        //CompleteTime = new DateTime(2013, 7, 28, 14, 25, 34),
                        SagaId = Guid.NewGuid(),
                        Changes = RestSharp.SimpleJson.DeserializeObject<List<SagaUpdate>>(sagaDataText) 
                    };
        }


        public async void Handle(SelectedMessageChanged @event)
        {
            var message = @event.Message;
            if (message != null)
            {
                if (message.Sagas != null)
                {
                    var originatingSaga = message.Sagas.FirstOrDefault();
                    if (originatingSaga != null)
                    {

                        _eventAggregator.Publish(new WorkStarted("Loading message body..."));

                        //CreateMockSaga();
                        if (Data == null || Data.SagaId != originatingSaga.SagaId)
                        {
                            Data = await _serviceControl.GetSagaById(originatingSaga.SagaId.ToString());

                            if (Data != null && Data.Changes != null)
                            {
                                ProcessDataValues(Data.Changes);
                            }
                            else
                            {
                                Data = null;
                            }
                        }

                        if (showMessageData)
                        {
                            RefreshMessageProperties();
                        }

                        RefreshShowData();

                        _eventAggregator.Publish(new WorkFinished());
                    }
                    else
                    {
                        Data = null;
                    }
                }
                else
                {
                    Data = null;
                }
            }
            else
            {
                Data = null;
            }
        }

        private void ProcessDataValues(List<SagaUpdate> list)
        {
            IList<SagaUpdatedValue> oldValues = new List<SagaUpdatedValue>();
            foreach (var change in list)
            {
                ProcessChange(oldValues, change.Values);
                oldValues = change.Values;
            }
        }

        private void ProcessChange(IList<SagaUpdatedValue> oldValues, IList<SagaUpdatedValue> newValues)
        {
            foreach (var value in newValues)
            {
                var oldValue = oldValues.FirstOrDefault(v => v.Name == value.Name);
                if (oldValue != null)
                {
                    value.OldValue = oldValue.NewValue;
                }
                else
                {
                    value.OldValue = string.Empty;
                }
            }
        }

        public bool HasSaga
        {
            get
            {
                return Data != null;
            }
        }

        public SagaData Data { get; set; }

        private bool showEndpoints = false;
        public bool ShowEndpoints
        {
            get 
            { 
                return showEndpoints; 
            }
            set
            { 
                showEndpoints = value; 
                NotifyOfPropertyChange(() => ShowEndpoints); 
            }
        }

        private bool showMessageData = false;
        public bool ShowMessageData
        {
            get
            {
                return showMessageData;
            }
            set
            {
                showMessageData = value;
                RefreshShowData();
                NotifyOfPropertyChange(() => ShowMessageData);
            }
        }

        public void ShowFlow()
        {
            _eventAggregator.Publish(new SwitchToFlowWindow());
        }

    }

    public interface ISagaWindowViewModel : IScreen
    {
        bool ShowMessageData { get; }
        bool ShowEndpoints { get; }
        SagaData Data { get; }
        void ShowFlow();
    }
}
