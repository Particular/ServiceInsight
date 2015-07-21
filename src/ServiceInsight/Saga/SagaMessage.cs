namespace Particular.ServiceInsight.Desktop.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Framework;
    using Models;
    using ServiceControl;

    public class SagaMessageDataItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SagaMessage : PropertyChangedBase
    {
        public string MessageId { get; set; }

        public bool IsPublished { get; set; }

        public MessageIntent Intent { get; set; }

        public bool IsEventMessage
        {
            get { return Intent == MessageIntent.Publish; }
        }

        public bool IsCommandMessage
        {
            get { return !IsEventMessage && !IsTimeout; }
        }

        public virtual bool IsTimeout
        {
            get
            {
                return IsSagaTimeoutMessage;
            }
        }

        public bool IsSagaTimeoutMessage { get; set; } //for SC, not to be confused with timeout outgoing messages

        public string MessageType
        {
            get;
            set;
        }

        public string MessageFriendlyTypeName
        {
            get { return TypeHumanizer.ToName(MessageType); }
        }

        DateTime? timeSent;
        public DateTime? TimeSent
        {
            get { return timeSent; }
            set
            {
                if (value == DateTime.MinValue)
                    timeSent = null;
                else
                    timeSent = value;

            }
        }

        public string ReceivingEndpoint { get; set; }

        public string OriginatingEndpoint { get; set; }

        MessageStatus status;

        public MessageStatus Status
        {
            get { return status == 0 ? MessageStatus.Successful : status; }
            set { status = value; }
        }

        public bool IsSelected { get; set; }

        bool showData;

        public bool ShowData
        {
            get { return showData && Data != null && Data.Any(); }
            set { showData = value; }
        }

        public IEnumerable<SagaMessageDataItem> Data { get; private set; }

        internal void RefreshData(IServiceControl serviceControl)
        {
            if (Data != null)
            {
                return;
            }

            Data = serviceControl.GetMessageData(this).Select(kvp => new SagaMessageDataItem { Key = kvp.Key, Value = kvp.Value }).ToList();
        }
    }

    public class SagaTimeoutMessage : SagaMessage
    {
        public SagaTimeoutMessage()
        {
            DeliverAt = DateTime.MinValue;
            Timeout = TimeSpan.MinValue;
        }

        public TimeSpan Timeout { get; set; }

        public override bool IsTimeout
        {
            get { return (DeliverAt != DateTime.MinValue || Timeout != TimeSpan.MinValue); }
        }

        public DateTime DeliverAt { get; set; }

        public string DeliveryDelay
        {
            get { return Timeout.ToString(@"hh\:mm\:ss"); }
            set { Timeout = TimeSpan.Parse(value); }
        }

        public string TimeoutFriendly
        {
            get
            {
                if (Timeout != TimeSpan.MinValue)
                    return string.Format("{0}{1}{2}{3}", GetFriendly(Timeout.Days, "d"), GetFriendly(Timeout.Hours, "h"), GetFriendly(Timeout.Minutes, "m"), GetFriendly(Timeout.Seconds, "s"));

                return DeliverAt.ToString();
            }
        }

        string GetFriendly(int time, string text)
        {
            if (time > 0)
            {
                return string.Format("{0}{1}", time, text);
            }
            return string.Empty;
        }
    }
}