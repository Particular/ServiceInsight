namespace Particular.ServiceInsight.Desktop.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using System.Xml;
    using Caliburn.Micro;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Particular.ServiceInsight.Desktop.ExtensionMethods;
    using Particular.ServiceInsight.Desktop.Framework;
    using Particular.ServiceInsight.Desktop.Models;
    using Particular.ServiceInsight.Desktop.ServiceControl;
    using Formatting = Newtonsoft.Json.Formatting;

    public class SagaMessageDataItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SagaMessage : PropertyChangedBase
    {
        public SagaMessage()
        {
          Viewer = new ContentViewer();
          ShowEntireContentCommand = this.CreateCommand(()=> { Viewer.Visible = true; });
        }

        public ICommand ShowEntireContentCommand { get; set; }

        public ContentViewer Viewer { get; private set; }

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

        public string MessageType
        {
            get;
            set;
        }

        public string MessageFriendlyTypeName
        {
            get { return TypeHumanizer.ToName(MessageType); }
        }

        public DateTime TimeSent { get; set; }

        public string ReceivingEndpoint { get; set; }

        public string OriginatingEndpoint { get; set; }

        MessageStatus status;

        public MessageStatus Status
        {
            get { return status == 0 ? MessageStatus.Successful : status; }
            set { status = value; }
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
            get { return (Status == MessageStatus.Failed) || (Status == MessageStatus.RepeatedFailure); }
        }

        public string StatusText
        {
            get { return statuses.FirstOrDefault(k => k.Key == Status).Value; }
            set { Status = statuses.FirstOrDefault(k => k.Value == value).Key; }
        }

        public bool HasRetried
        {
            get { return Status == MessageStatus.RetryIssued; }
        }

        bool showData;

        public bool ShowData
        {
            get { return showData && Data != null && Data.Count > 0; }
            set { showData = value; }
        }

        public IList<SagaMessageDataItem> Data { get; private set; }

        internal void RefreshData(IServiceControl serviceControl)
        {
            if (Data != null)
            {
                return;
            }

            Data = new List<SagaMessageDataItem>();

            var tuple = serviceControl.GetMessageData(this);

            if (tuple == null)
            {
                return;
            }

            var messageData = tuple.Item2;
            var messageBodyFormatted = String.Empty;
            var parsable = true;
            var isJsonFromXml = false;
            if (tuple.Item1 == "text/xml" || tuple.Item1 == "application/xml")
            {
                try
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(tuple.Item2);
                    messageBodyFormatted = doc.GetFormatted();
                    messageData = JsonConvert.SerializeXmlNode(doc.DocumentElement, Formatting.Indented);
                    isJsonFromXml = true;
                }
                catch (Exception)
                {
                    parsable = false;
                }
            }
            else if (!(tuple.Item1 == "text/json" || tuple.Item1 == "application/json"))
            {
                parsable = false;
            }
            
            if (parsable)
            {
                JObject jObject = null;
                try
                {
                    jObject = JObject.Parse(messageData);
                }
                catch (JsonReaderException)
                {
                    //Ignore, we couldn't parse the json, something must be wrong with it
                }

                if (jObject != null && jObject.HasValues)
                {
                    if (isJsonFromXml)
                    {
                        var rootElement = (JProperty)jObject.First;

                        if (rootElement.HasValues)
                        {
                            jObject = (JObject)rootElement.Value;
                            PopulateData(jObject, true);
                        }
                    }
                    else
                    {
                        messageBodyFormatted = jObject.GetFormatted();
                        PopulateData(jObject);
                    }
                }
            }

            if (isJsonFromXml)
            {
                Viewer.SyntaxHighlighting = "XML";
            }
            Viewer.DisplayTitle = MessageType;
            Viewer.Data = messageBodyFormatted;
        }

        void PopulateData(JObject jObject, bool removeNamespaces = false)
        {
            foreach (var prop in jObject.Properties())
            {
                if (removeNamespaces && prop.Name.StartsWith("@xmlns"))
                {
                    continue;
                }
                Data.Add(new SagaMessageDataItem
                {
                    Key = prop.Name,
                    Value = prop.Value.ToString()
                });
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
