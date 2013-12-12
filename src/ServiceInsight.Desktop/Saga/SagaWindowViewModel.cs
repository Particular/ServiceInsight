using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Screens;
using NServiceBus.Profiler.Desktop.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaWindowViewModel : Screen, ISagaWindowViewModel
    {
        private ISagaWindowView _view;
        private IEventAggregator _eventAggregator;

        public SagaWindowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public override void AttachView(object view, object context)
        {
            base.AttachView(view, context);
            _view = (ISagaWindowView)view;

            CreateMockSaga();
        }

        private void CreateMockSaga()
        {
            this.Name = "ProcessOrderSaga";
            this.CompleteTime = new DateTime(2013, 7, 28, 14, 25, 34);
            this.Guid = Guid.NewGuid();
            var timeoutGuid = Guid.NewGuid();

            var sagaDataText = System.IO.File.ReadAllText("saga\\saga.data").Replace("\r", "").Replace("\n", "");
            this.Steps = RestSharp.SimpleJson.DeserializeObject<List<SagaStep>>(sagaDataText);
        }

        public string Name { get; private set; }

        public Guid Guid { get; private set; }

        public IEnumerable<SagaStep> Steps { get; private set; }

        public DateTime CompleteTime { get; private set; }

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

        public void ShowFlow()
        {
            _eventAggregator.Publish(new SwitchToFlowWindow());
        }

    }

    public interface ISagaWindowViewModel : IScreen
    {
        bool ShowEndpoints { get; }
        IEnumerable<SagaStep> Steps { get; }
        void ShowFlow();
    }
}
