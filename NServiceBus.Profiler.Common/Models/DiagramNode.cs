using System;

namespace NServiceBus.Profiler.Common.Models
{
    public class DiagramNode : StoredMessage
    {
        public DiagramNode(StoredMessage msg)
        {
            ConversationId = msg.ConversationId;
            CorrelationId = msg.CorrelationId;
            Destination = msg.Destination;
            FailureDetails = msg.FailureDetails;
            Id = msg.Id;
            IsDeferredMessage = msg.IsDeferredMessage;
            MessageType = msg.MessageType;
            OriginatingEndpoint = msg.OriginatingEndpoint;
            ReceivingEndpoint = msg.ReceivingEndpoint;
            RelatedToMessageId = msg.RelatedToMessageId;
            Status = msg.Status;
            TimeSent = msg.TimeSent;
            Statistics = msg.Statistics;
        }

        public string ShortMessageId
        {
            get
            {
                return String.Format("{0}...", Id.Substring(0, 7));
            }
        }

        public bool IsCurrentMessage { get; set; }
    }
}