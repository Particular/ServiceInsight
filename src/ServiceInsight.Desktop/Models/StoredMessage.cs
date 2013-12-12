using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NServiceBus.Profiler.Desktop.Models
{
    [DebuggerDisplay("Id={Id},MessageId={MessageId},RelatedToMessageId={RelatedToMessageId}")]
    public class StoredMessage : MessageBody
    {
        public MessageStatus Status { get; set; }
        public MessageIntent MessageIntent { get; set; }
        public Endpoint OriginatingEndpoint { get; set; }
        public Endpoint ReceivingEndpoint { get; set; }
        public MessageStatistics Statistics { get; set; }
        public string RelatedToMessageId { get; set; }
        public string ConversationId { get; set; }
        public string ContentType { get; set; }
        public string MessageId { get; set; }

        public List<StoredMessageHeader> Headers
        {
            get
            {
                if (headers == null)
                {
                    //todo: lazy load
                    headers = new List<StoredMessageHeader>();
                }

                return headers;
            }
            set
            {
                headers = value;
            }
        }

        private List<StoredMessageHeader> headers;

        public string GetHeaderByKey(string key)
        {
            //Note: Some keys start with NServiceBus, some don't
            var keyWithPrefix = "NServiceBus." + key;
            var pair = Headers.FirstOrDefault(x => x.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase) || 
                                                   x.Key.Equals(keyWithPrefix, StringComparison.InvariantCultureIgnoreCase));
            return pair == null ? string.Empty : pair.Value;
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
        public const string ExceptionType = "ExceptionInfo.ExceptionType";
        public const string ExceptionMessage = "ExceptionInfo.Message";
        public const string ExceptionSource = "ExceptionInfo.Source";
        public const string ExceptionStackTrace = "ExceptionInfo.StackTrace";
        public const string FailedQueue = "FailedQ";
        public const string TimeSent = "TimeSent";
        public const string TimeOfFailure = "TimeOfFailure";
    }

}