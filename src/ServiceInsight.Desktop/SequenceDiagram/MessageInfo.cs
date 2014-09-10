namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Framework;
    using Models;
    using ReactiveUI;

    public class MessageInfo : ReactiveObject, IHandle<MessageInfo.HiliteEvent>
    {
        public class HiliteEvent
        {
            public string MessageID { get; set; }
        }

        private readonly IEventAggregator eventAggregator;

        public MessageInfo(
            IEventAggregator eventAggregator,
            SequenceDiagramViewModel viewModel,
            StoredMessage message,
            ReactiveList<EndpointInfo> endpoints)
        {
            this.eventAggregator = eventAggregator;
            Message = message;
            Endpoints = endpoints;

            RetryMessageCommand = viewModel.RetryMessageCommand;
            CopyConversationIDCommand = viewModel.CopyConversationIDCommand;
            CopyMessageURICommand = viewModel.CopyMessageURICommand;
            SearchByMessageIDCommand = viewModel.SearchByMessageIDCommand;

            Name = message.FriendlyMessageType;
            if (message.Sagas != null && message.Sagas.Any())
                SagaName = TypeHumanizer.ToName(message.Sagas.First().SagaType);
            CriticalTime = message.CriticalTime;
            DeliveryTime = message.DeliveryTime;
            ProcessingTime = message.ProcessingTime;

            ExceptionType = message.GetHeaderByKey(MessageHeaderKeys.ExceptionType);
            ExceptionMessage = message.GetHeaderByKey(MessageHeaderKeys.ExceptionMessage);

            endpoints.Changed.Subscribe(_ => UpdateIndicies());

            UpdateIndicies();
        }

        public StoredMessage Message { get; private set; }

        public IEnumerable<EndpointInfo> Endpoints { get; set; }

        public bool Selected { get; set; }
        public bool Hilited { get; set; }
        public bool HiliteHandler { get; set; }

        public bool IsFirst { get; set; }

        public string Name { get; private set; }
        public string SagaName { get; private set; }

        public TimeSpan CriticalTime { get; private set; }
        public TimeSpan DeliveryTime { get; private set; }
        public TimeSpan ProcessingTime { get; private set; }

        public string ExceptionType { get; private set; }
        public string ExceptionMessage { get; private set; }

        public string SendingEndpoint
        {
            get { return Message.SendingEndpoint.Name; }
        }
        public string ReceivingEndpoint
        {
            get { return Message.ReceivingEndpoint.Name; }
        }

        public bool IsPublished
        {
            get { return Message.MessageIntent == MessageIntent.Publish; }
        }
        public bool IsTimeout
        {
            get
            {
                var isTimeoutString = Message.GetHeaderByKey(MessageHeaderKeys.IsSagaTimeout);
                return !string.IsNullOrEmpty(isTimeoutString) && bool.Parse(isTimeoutString);
            }
        }
        public bool IsFailed
        {
            get
            {
                var exceptionType = Message.GetHeaderByKey(MessageHeaderKeys.ExceptionType);
                return !string.IsNullOrEmpty(exceptionType);
            }
        }

        public bool IsCommand { get { return !IsTimeout && !IsPublished; } }
        public bool IsEvent { get { return !IsTimeout && IsPublished; } }

        public bool IsSagaInitiated
        {
            get
            {
                return string.IsNullOrEmpty(Message.GetHeaderByKey(MessageHeaderKeys.SagaId))
                    && !string.IsNullOrEmpty(Message.GetHeaderByKey(MessageHeaderKeys.OriginatedSagaId));
            }
        }

        public bool IsSagaCompleted
        {
            get
            {
                var status = Message.InvokedSagas == null ? null : Message.InvokedSagas.FirstOrDefault();
                return status != null && status.ChangeStatus == "Completed";
            }
        }

        public int SendingEndpointIndex { get; private set; }
        public int ReceivingEndpointIndex { get; private set; }

        public int MinEndpointIndex { get; private set; }
        public int MaxEndpointIndex { get; private set; }
        public int MessagePopupIndex { get; private set; }

        public ICommand RetryMessageCommand { get; private set; }
        public ICommand CopyConversationIDCommand { get; private set; }
        public ICommand CopyMessageURICommand { get; private set; }
        public ICommand SearchByMessageIDCommand { get; private set; }

        private void OnHilitedChanged()
        {
            eventAggregator.Publish(new HiliteEvent { MessageID = Hilited ? Message.RelatedToMessageId : "" });
        }

        private void OnEndpointsChanged()
        {
            UpdateIndicies();
        }

        private void OnSendingEndpointChanged()
        {
            UpdateIndicies();
        }

        private void OnReceivingEndpointChanged()
        {
            UpdateIndicies();
        }

        private static int FindEndpointIndex(IEnumerable<EndpointInfo> endpoints, string endpointName)
        {
            var i = 0;
            foreach (var endpoint in endpoints)
            {
                if (string.Equals(endpointName, endpoint.FullName, StringComparison.InvariantCultureIgnoreCase))
                    return i;
                i++;
            }
            return -1;
        }

        private void UpdateIndicies()
        {
            if (Endpoints == null)
                return;

            SendingEndpointIndex = FindEndpointIndex(Endpoints, SendingEndpoint);
            ReceivingEndpointIndex = FindEndpointIndex(Endpoints, ReceivingEndpoint);

            MinEndpointIndex = Math.Min(ReceivingEndpointIndex, SendingEndpointIndex);
            MaxEndpointIndex = Math.Max(ReceivingEndpointIndex, SendingEndpointIndex);

            MessagePopupIndex = MinEndpointIndex + (MaxEndpointIndex - MinEndpointIndex) / 2;
        }

        public void Handle(HiliteEvent message)
        {
            HiliteHandler = message.MessageID == Message.MessageId;
        }
    }
}