using Caliburn.PresentationFramework;
using NServiceBus.Profiler.Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Desktop.Saga
{
    public class SagaMessage : PropertyChangedBase
    {
        public Guid MessageId { get; set; }
        public bool IsPublished { get; set; }

        public bool IsSagaTimeoutMessage { get; set; } //for SC, not to be confused with timeout outgoing messages

        private string messageType;
        public string MessageType 
        { 
            get
            {
                return ProcessType(messageType);
            }
            set
            {
                messageType = value;
            }
        }

        private string ProcessType(string messageType)
        {
            if (string.IsNullOrEmpty(messageType))
                return string.Empty;

            var clazz = messageType.Split(',').First();
            var objectName = clazz.Split('.').Last();

            if (objectName.Contains("+"))
                objectName = objectName.Split('+').Last();

            return objectName;
        }

        public DateTime TimeSent { get; set; }
        public string ReceivingEndpoint { get; set; }
        public string OriginatingEndpoint { get; set; }

        private MessageStatus status;
        public MessageStatus Status
        {
            get
            {
                return status == 0 ? MessageStatus.Successful : status;
            }
            set 
            {
                status = value;
            }
        }

        private List<KeyValuePair<MessageStatus, string>> stati = new List<KeyValuePair<MessageStatus, string>> { 
            new KeyValuePair<MessageStatus, string>(MessageStatus.Failed, "Fail" ),
            new KeyValuePair<MessageStatus, string>(MessageStatus.RepeatedFailure, "RepeatedFail" ),
            new KeyValuePair<MessageStatus, string>(MessageStatus.RetryIssued, "Retry" ),
            new KeyValuePair<MessageStatus, string>(MessageStatus.Successful, "Success" ),
        };

        private bool isSelected = false;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public bool HasFailed
        {
            get
            {
                return (Status == MessageStatus.Failed) || (Status == MessageStatus.RepeatedFailure);
            }
        }

        public string StatusText
        {
            get
            {
                return stati.FirstOrDefault(k => k.Key == Status).Value;
            }
            set
            {
                Status = stati.FirstOrDefault(k => k.Value == value).Key;
            }
        }

        public bool HasRetried
        {
            get
            {
                return Status == MessageStatus.RetryIssued;
            }
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

        public bool IsTimeout
        {
            get
            {
                return (DeliverAt != DateTime.MinValue || Timeout != TimeSpan.MinValue);
            }
        }

        public DateTime DeliverAt { get; set; }

        public string DeliveryDelay
        {
            get
            {
                return Timeout.ToString(@"hh\:mm\:ss");
            }
            set
            {
                Timeout = TimeSpan.Parse(value);
            }
        }

        public string TimeoutFriendly
        {
            get
            {
                if (Timeout != TimeSpan.MinValue)
                    return string.Format("{0}{1}{2}", GetFriendly(Timeout.Hours, "h"), GetFriendly(Timeout.Minutes, "m"), GetFriendly(Timeout.Seconds, "s"));

                return DeliverAt.ToString();
            }
        }

        private string GetFriendly(int time, string text)
        {
            if (time > 0)
            {
                return string.Format("{0}{1}", time, text);
            }
            return string.Empty;
        }
    }
}
