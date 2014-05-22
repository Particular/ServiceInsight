namespace Particular.ServiceInsight.Desktop.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Caliburn.Micro;
    using Models;
    using ServiceControl;

    public class SagaMessage : PropertyChangedBase
    {
        public Guid MessageId { get; set; }

        public bool IsPublished { get; set; }

        public virtual bool IsTimeout
        {
            get
            {
                return IsSagaTimeoutMessage;
            }
        }

        public bool IsSagaTimeoutMessage { get; set; } //for SC, not to be confused with timeout outgoing messages

        string messageType;

        public string MessageType
        {
            get
            {
                return ProcessType();
            }
            set
            {
                messageType = value;
            }
        }

        string ProcessType()
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

        MessageStatus status;

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

        List<KeyValuePair<MessageStatus, string>> statuses = new List<KeyValuePair<MessageStatus, string>> {
            new KeyValuePair<MessageStatus, string>(MessageStatus.Failed, "Fail" ),
            new KeyValuePair<MessageStatus, string>(MessageStatus.RepeatedFailure, "RepeatedFail" ),
            new KeyValuePair<MessageStatus, string>(MessageStatus.RetryIssued, "Retry" ),
            new KeyValuePair<MessageStatus, string>(MessageStatus.Successful, "Success" ),
        };

        public bool IsSelected { get; set; }

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
                return statuses.FirstOrDefault(k => k.Key == Status).Value;
            }
            set
            {
                Status = statuses.FirstOrDefault(k => k.Value == value).Key;
            }
        }

        public bool HasRetried
        {
            get
            {
                return Status == MessageStatus.RetryIssued;
            }
        }

        bool showData;

        public bool ShowData
        {
            get
            {
                return showData && Data != null && Data.Any();
            }
            set
            {
                showData = value;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Data { get; private set; }

        internal async Task RefreshData(DefaultServiceControl serviceControl)
        {
            //TODO: Consider moving this into ServiceControl e.g. GetSageMessageBody or something, models should be just about data
            if (Data != null) return;

            var url = string.Format("/messages/{0}/body", MessageId);
            var bodyString = await serviceControl.GetBody(url);
            if (bodyString != null)
            {
                if (IsXml(bodyString))
                {
                    Data = GetXmlData(bodyString.Replace("\\\"", "\"").Replace("\\r", "\r").Replace("\\n", "\n"));
                }
                else
                {
                    Data = JsonPropertiesHelper.ProcessValues(bodyString, CleanupBodyString);
                }
            }
            else
            {
                Data = new List<KeyValuePair<string, string>>();
            }
        }

        IEnumerable<KeyValuePair<string, string>> GetXmlData(string bodyString)
        {
            try
            {
                var xml = XDocument.Parse(bodyString);
                if (xml.Root != null)
                {
                    var root = xml.Root.Nodes().FirstOrDefault() as XElement;
                    if (root != null)
                    {
                        return root.Nodes()
                                   .OfType<XElement>()
                                   .Select(n => new KeyValuePair<string, string>(n.Name.LocalName, n.Value));
                    }
                }
            }
            catch (XmlException) { }
            return new List<KeyValuePair<string, string>>();
        }

        static bool IsXml(string bodyString)
        {
            return bodyString.StartsWith("<?xml");
        }

        static string CleanupBodyString(string bodyString)
        {
            return bodyString.Replace("\u005c", string.Empty).Replace("\uFEFF", string.Empty).TrimStart("[\"".ToCharArray()).TrimEnd("]\"".ToCharArray());
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