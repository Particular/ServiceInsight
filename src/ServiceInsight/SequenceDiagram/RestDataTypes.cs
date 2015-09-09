namespace ServiceInsight.SequenceDiagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Particular.ServiceInsight.Desktop.Models;

    public class EndpointAddress
    {
        public string name { get; set; }
        public string host_id { get; set; }
        public string host { get; set; }
    }

    [DebuggerDisplay("{value}", Name = "{key}")]
    public class Header
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class Saga
    {
        public string saga_type { get; set; }
        public string saga_id { get; set; }
    }

    public class ReceivedMessage
    {
        public string id { get; set; }
        public string message_id { get; set; }
        public string message_type { get; set; }
        public EndpointAddress sending_endpoint { get; set; }
        public EndpointAddress receiving_endpoint { get; set; }
        public DateTime? time_sent { get; set; }
        public DateTime processed_at { get; set; }
        public TimeSpan critical_time { get; set; }
        public TimeSpan processing_time { get; set; }
        public TimeSpan delivery_time { get; set; }
        public bool is_system_message { get; set; }
        public string conversation_id { get; set; }
        public List<Header> headers { get; set; }
        public MessageStatus status { get; set; }
        public MessageIntent message_intent { get; set; }
        public string body_url { get; set; }
        public int body_size { get; set; }
        public List<Saga> invoked_sagas { get; set; }
        public Saga originates_from_saga { get; set; }
    }

}
