namespace Particular.ServiceInsight.Desktop.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using ReactiveUI;

    public class MessageInfo : ReactiveObject
    {
        private readonly StoredMessage message;

        public MessageInfo(StoredMessage message, ReactiveList<EndpointInfo> endpoints)
        {
            this.message = message;
            Endpoints = endpoints;

            Name = message.FriendlyMessageType;
            if (message.Sagas != null && message.Sagas.Any())
                SagaName = message.Sagas.First().SagaType;
            CriticalTime = message.CriticalTime;
            DeliveryTime = message.DeliveryTime;
            ProcessingTime = message.ProcessingTime;

            endpoints.Changed.Subscribe(_ => UpdateIndicies());

            UpdateIndicies();
        }

        public IEnumerable<EndpointInfo> Endpoints { get; set; }

        public bool Selected { get; set; }

        public bool IsFirst { get; set; }

        public string Name { get; private set; }
        public string SagaName { get; private set; }

        public TimeSpan CriticalTime { get; private set; }
        public TimeSpan DeliveryTime { get; private set; }
        public TimeSpan ProcessingTime { get; private set; }

        public string SendingEndpoint
        {
            get { return message.SendingEndpoint.Name; }
        }
        public string ReceivingEndpoint
        {
            get { return message.ReceivingEndpoint.Name; }
        }

        public bool IsPublished
        {
            get { return message.MessageIntent == MessageIntent.Publish; }
        }
        public bool IsTimeout
        {
            get
            {
                var isTimeoutString = message.GetHeaderByKey(MessageHeaderKeys.IsSagaTimeout);
                return !string.IsNullOrEmpty(isTimeoutString) && bool.Parse(isTimeoutString);
            }
        }

        public bool IsCommand { get { return !IsTimeout && !IsPublished; } }
        public bool IsEvent { get { return !IsTimeout && IsPublished; } }

        public bool IsSagaInitiated
        {
            get
            {
                return string.IsNullOrEmpty(message.GetHeaderByKey(MessageHeaderKeys.SagaId))
                    && !string.IsNullOrEmpty(message.GetHeaderByKey(MessageHeaderKeys.OriginatedSagaId));
            }
        }

        public bool IsSagaCompleted
        {
            get
            {
                var status = message.InvokedSagas == null ? null : message.InvokedSagas.FirstOrDefault();
                return status != null && status.ChangeStatus == "Completed";
            }
        }

        public int SendingEndpointIndex { get; private set; }
        public int ReceivingEndpointIndex { get; private set; }

        public int MinEndpointIndex { get; private set; }
        public int MaxEndpointIndex { get; private set; }
        public int MessagePopupIndex { get; private set; }

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
            SendingEndpointIndex = FindEndpointIndex(Endpoints, SendingEndpoint);
            ReceivingEndpointIndex = FindEndpointIndex(Endpoints, ReceivingEndpoint);

            MinEndpointIndex = Math.Min(ReceivingEndpointIndex, SendingEndpointIndex);
            MaxEndpointIndex = Math.Max(ReceivingEndpointIndex, SendingEndpointIndex);

            MessagePopupIndex = MinEndpointIndex + (MaxEndpointIndex - MinEndpointIndex) / 2;
        }
    }
}