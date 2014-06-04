namespace Particular.ServiceInsight.Desktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using ExtensionMethods;

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
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public TimeSpan DeliveryTime { get; set; }
        public string ConversationId { get; set; }

        public string ElapsedCriticalTime
        {
            get
            {
                return CriticalTime.GetElapsedTime();
            }
        }

        public string ElapsedProcessingTime
        {
            get
            {
                return ProcessingTime.GetElapsedTime();
            }
        }

        public MessageStatistics Statistics
        {
            get
            {
                if (statistics == null)
                {
                    statistics = new MessageStatistics
                    {
                        CriticalTime = CriticalTime,
                        ProcessingTime = ProcessingTime
                    };
                }
                return statistics;
            }
            set
            {
                statistics = value;
            }
        }

        MessageStatistics statistics;

        public string ElapsedDeliveryTime
        {
            get
            {
                return DeliveryTime.GetElapsedTime();
            }
        }

        public string RelatedToMessageId
        {
            get
            {
                return GetHeaderByKey("NServiceBus.RelatedTo");
            }
        }

        public string ContentType
        {
            get
            {
                return GetHeaderByKey("NServiceBus.ContentType");
            }
        }

        public string MessageId { get; set; }


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
                    return new List<SagaInfo> { OriginatesFromSaga };

                return null;
            }
        }

        public List<SagaInfo> InvokedSagas{get;set;}

        public SagaInfo OriginatesFromSaga{get;set;}

        public string GetURIQuery()
        {
            return string.Format("?EndpointName={0}&Search={1}", ReceivingEndpoint.Name, MessageId);
        }

        public string GetHeaderByKey(string key)
        {
            //Note: Some keys start with NServiceBus, some don't
            var keyWithPrefix = "NServiceBus." + key;
            var pair = Headers.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) ||
                                                   x.Key.Equals(keyWithPrefix, StringComparison.InvariantCultureIgnoreCase));
            return pair == null ? string.Empty : pair.Value;
        }

        public bool DisplayPropertiesChanged(StoredMessage focusedMessage)
        {
            if (focusedMessage == null) return true;

            return (Status != focusedMessage.Status) ||
                   (TimeSent != focusedMessage.TimeSent) ||
                   (ProcessingTime != focusedMessage.ProcessingTime) ||
                   (ReceivingEndpoint.ToString() != focusedMessage.ReceivingEndpoint.ToString()) ||
                   (SendingEndpoint.ToString() != focusedMessage.SendingEndpoint.ToString());
        }
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


