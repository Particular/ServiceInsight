namespace ServiceInsight.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    [DebuggerDisplay("Id={Id},MessageId={MessageId},RelatedToMessageId={RelatedToMessageId}")]
    public class StoredMessage : MessageBody
    {
        public StoredMessage()
        {
            Headers = new List<StoredMessageHeader>();
        }

        public MessageStatus Status { get; set; }

        public MessageIntent MessageIntent { get; set; }

        public Endpoint SendingEndpoint { get; set; }

        public Endpoint ReceivingEndpoint { get; set; }

        public TimeSpan ProcessingTime { get; set; }

        public DateTime ProcessedAt { get; set; }

        public string ConversationId { get; set; }

        public string RelatedToMessageId => GetHeaderByKey(MessageHeaderKeys.RelatedTo);

        public string ContentType => GetHeaderByKey(MessageHeaderKeys.ContentType);

        public string ExceptionMessage => GetHeaderByKey(MessageHeaderKeys.ExceptionMessage);

        public string ExceptionType => GetHeaderByKey(MessageHeaderKeys.ExceptionType);

        public string MessageId { get; set; }

        public string InstanceId { get; set; }

        public List<StoredMessageHeader> Headers
        {
            get;
            set;
        }

        public List<SagaInfo> Sagas
        {
            get
            {
                if (InvokedSagas != null)
                {
                    if (OriginatesFromSaga != null)
                    {
                        return InvokedSagas.Union(new List<SagaInfo> { OriginatesFromSaga }).ToList();
                    }
                    return InvokedSagas;
                }

                if (OriginatesFromSaga != null)
                {
                    return new List<SagaInfo> { OriginatesFromSaga };
                }

                return null;
            }
        }

        public List<SagaInfo> InvokedSagas { get; set; }

        public SagaInfo OriginatesFromSaga { get; set; }

        public string GetURIQuery() => string.Format("?EndpointName={0}&Search={1}", ReceivingEndpoint.Name, MessageId);

        public string GetHeaderByKey(string key, string defaultValue = "")
        {
            //NOTE: Some keys start with NServiceBus, some don't
            var keyWithPrefix = "NServiceBus." + key;
            var pair = Headers.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) ||
                                                   x.Key.Equals(keyWithPrefix, StringComparison.InvariantCultureIgnoreCase));
            return pair == null ? defaultValue : pair.Value;
        }

        public bool DisplayPropertiesChanged(StoredMessage focusedMessage) => (focusedMessage == null) ||
       (Status != focusedMessage.Status) ||
       (TimeSent != focusedMessage.TimeSent) ||
       (ProcessingTime != focusedMessage.ProcessingTime) ||
       (ReceivingEndpoint != focusedMessage.ReceivingEndpoint) ||
       (SendingEndpoint != focusedMessage.SendingEndpoint);
    }

    [DebuggerDisplay("Key={Key},Value={Value}")]
    public class StoredMessageHeader
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }

    [DebuggerDisplay("SagaType={SagaType},SagaId={Value}")]
    public class SagaInfo
    {
        public string ChangeStatus { get; set; }

        public string SagaType { get; set; }

        public Guid SagaId { get; set; }
    }

    public class MessageHeaderKeys
    {
        public const string Version = "Version";
        public const string EnclosedMessageTypes = "EnclosedMessageTypes";
        public const string Retries = "Retries";
        public const string RelatedTo = "RelatedTo";
        public const string ContentType = "ContentType";
        public const string IsDeferedMessage = "IsDeferedMessage";
        public const string ConversationId = "ConversationId";
        public const string MessageId = "MessageId";
        public const string ExceptionType = "ExceptionInfo.ExceptionType";
        public const string ExceptionMessage = "ExceptionInfo.Message";
        public const string ExceptionSource = "ExceptionInfo.Source";
        public const string ExceptionStackTrace = "ExceptionInfo.StackTrace";
        public const string FailedQueue = "FailedQ";
        public const string TimeSent = "TimeSent";
        public const string TimeOfFailure = "TimeOfFailure";
        public const string IsSagaTimeout = "IsSagaTimeoutMessage";
        public const string SagaId = "SagaId";
        public const string OriginatedSagaId = "OriginatingSagaId";
        public const string SagaStatus = "ServiceControl.SagaChangeStatus";
    }
}