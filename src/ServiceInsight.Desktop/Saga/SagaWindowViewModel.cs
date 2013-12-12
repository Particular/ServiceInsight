using Caliburn.PresentationFramework.Screens;
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
            this.Steps = new List<SagaStep> { 
                new SagaStep { IsFirstNode = true, IsTimeout = false, 
                        StartingMessage = new SagaMessage { Id = Guid.NewGuid(), IsPublished = true, Name = "SubmitOrder", Time = new DateTime(2013, 7, 28, 14, 12, 17), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host" },
                        Values = new List<SagaUpdatedValue> { new SagaUpdatedValue { Name = "VeryLongValue", NewValue = "Yup" }, new SagaUpdatedValue { Name = "Location", NewValue = "NY", OldValue = "CA" }, new SagaUpdatedValue { Name = "Phone", NewValue = "555 - 5648" } },
                        TimeoutMessages = new List<SagaTimeoutMessage> { new SagaTimeoutMessage { IsPublished = true, Time = new DateTime(2013, 7, 28, 14, 12, 37), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host", Name = "BuyersRemorseIsOver", Id = timeoutGuid, Timeout = new TimeSpan(0, 0, 20) }, new SagaTimeoutMessage { IsPublished = true, Time = new DateTime(2013, 7, 28, 14, 12, 37), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host", Name = "BuyersRemorseIsOver", Id = Guid.NewGuid(), Timeout = new TimeSpan(0, 0, 20) } } }
                , new SagaStep { IsFirstNode = false, IsTimeout = false, 
                        StartingMessage = new SagaMessage { Id = Guid.NewGuid(), IsPublished = true, Name = "SubmitOrderUpdate", Time = new DateTime(2013, 7, 28, 14, 23, 34), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host" },
                        Values = new List<SagaUpdatedValue> { new SagaUpdatedValue { Name = "Location", NewValue = "NY", OldValue = "CA" }, new SagaUpdatedValue { Name = "Phone", NewValue = "555 - 2140", OldValue = "555 - 5648" } } ,
                        Messages = new List<SagaMessage> { new SagaMessage { Id = Guid.NewGuid(), IsPublished = true, Name = "OrderUpdated", Time = new DateTime(2013, 7, 28, 14, 23, 34), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host" }, new SagaMessage { Id = Guid.NewGuid(), IsPublished = true, Name = "OrderUpdated", Time = new DateTime(2013, 7, 28, 14, 23, 34), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host"  }  } }
                , new SagaStep { IsFirstNode = false, IsTimeout = true, 
                        StartingMessage = new SagaMessage { Id = timeoutGuid, IsPublished = true, Name = "BuyersRemorseIsOver", Time = new DateTime(2013, 7, 28, 14, 12, 37), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host" },
                        Values = new List<SagaUpdatedValue> { new SagaUpdatedValue { Name = "Location", NewValue = "FL", OldValue = "NY" }, new SagaUpdatedValue { Name = "Phone", NewValue = "555 - 5648", OldValue = "555 - 5648" } } ,
                        Messages = new List<SagaMessage> { new SagaMessage { Id = Guid.NewGuid(), IsPublished = false, Name = "OrderAccepted", Time = new DateTime(2013, 7, 28, 14, 12, 37), OriginatingEndpoint = "endpoint@host", ReceivingEndpoint = "endpoint@host"  }  } }
            };
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

    }

    public interface ISagaWindowViewModel : IScreen
    {
        bool ShowEndpoints { get; }
        IEnumerable<SagaStep> Steps { get; }
    }
}
