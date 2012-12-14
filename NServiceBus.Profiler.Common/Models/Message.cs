using System;

namespace NServiceBus.Profiler.Common.Models
{
    [Serializable]
    public class MessageBody : MessageInfo
    {
        [NonSerialized]
        public static MessageBody Empty;

        static MessageBody()
        {
            Empty = new MessageBody
            {
                Content = new byte[0],
                Headers = new byte[0],
                CorrelationId = string.Empty,
                TransactionId = string.Empty,
                Destination = Queue.Empty,
                Response = Queue.Empty
            };
        }

        public MessageBody()
        {
        }

        public MessageBody(string id, string label, DateTime sentAt) 
            : base(id, label, sentAt)
        {
        }

        public byte[] Content { get; set; }
        public byte[] Headers { get; set; }
        public string CorrelationId { get; set; }
        public string TransactionId { get; set; }
        public Queue Destination { get; set; }
        public Queue Response { get; set; }
    }

    
}