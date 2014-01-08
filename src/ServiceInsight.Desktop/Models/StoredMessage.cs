using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NServiceBus.Profiler.Desktop.ExtensionMethods;

namespace NServiceBus.Profiler.Desktop.Models
{
    [DebuggerDisplay("Id={Id},MessageId={MessageId},RelatedToMessageId={RelatedToMessageId}")]
    public class StoredMessage : MessageBody
    {
        private MessageStatistics _statistics;
        private List<StoredMessageHeader> _headers;

        public MessageStatus Status { get; set; }
        public MessageIntent MessageIntent { get; set; }
        public Endpoint SendingEndpoint { get; set; }

        public Endpoint ReceivingEndpoint { get; set; }
        public TimeSpan CriticalTime { get; set; }
        public TimeSpan ProcessingTime { get; set; }
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
                if (_statistics == null)
                {
                    _statistics = new MessageStatistics
                    {
                        CriticalTime = CriticalTime,
                        ProcessingTime = ProcessingTime
                    };
                }
                return _statistics;
            }
            set
            {
                _statistics = value;
            }
        }

        public string OriginatingSagaType
        {
            get
            {
                return GetHeaderByKey("OriginatingSagaType");
            }
        }

        public string OriginatingSagaId
        {
            get
            {
                return GetHeaderByKey("OriginatingSagaId");
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
            get
            {
                if (_headers == null)
                {
                    //todo: lazy load
                    _headers = new List<StoredMessageHeader>();
                }

                return _headers;
            }
            set
            {
                _headers = value;
            }
        }

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
        public const string SagaId = "SagaId";
        public const string OriginatingSagaId = "OriginatingSagaId";
        public const string SagaType = "SagaType";
    }

}