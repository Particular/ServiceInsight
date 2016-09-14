namespace ServiceInsight.Models
{
    using System;
    using Framework;
    using Pirac;

    [Serializable]
    public class MessageInfo : BindableObject
    {
        public string Id { get; set; }

        public string Label { get; set; }

        public string MessageType { get; set; }

        public DateTime? TimeSent { get; set; }

        public string FriendlyMessageType => TypeHumanizer.ToName(MessageType);
    }
}