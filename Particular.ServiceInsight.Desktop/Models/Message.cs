using System;
using Particular.ServiceInsight.Desktop.Models;

namespace Particular.ServiceInsight.Desktop.Models
{
    [Serializable]
    public class MessageBody : MessageInfo
    {
        public MessageBody()
        {
        }

        public MessageBody(string id, string label, DateTime sentAt) 
            : base(id, label, sentAt)
        {
        }

        public byte[] BodyRaw { get; set; }
        public string Body { get; set; }
        public byte[] Headers { get; set; }
        public string CorrelationId { get; set; }
        public string TransactionId { get; set; }
        public Queue Destination { get; set; }
        public Queue Response { get; set; }
    }
}