using System;
using RestSharp.Deserializers;

namespace NServiceBus.Profiler.Desktop.Models
{
    [Serializable]
    public class MessageBody : MessageInfo
    {
        public MessageBody()
        {
            BodyRaw = new byte[0];
            HeaderRaw = new byte[0];
        }

        public MessageBody(string id, string label, DateTime sentAt) 
            : base(id, label, sentAt)
        {
        }

        [DeserializeAs(Name = "Headers")]
        public byte[] HeaderRaw { get; set; }
        public byte[] BodyRaw { get; set; }
        public string Body { get; set; }
    }
}