namespace ServiceInsight.Models
{
    using System;
    using Caliburn.Micro;
    using Framework;

    [Serializable]
    public class MessageInfo : PropertyChangedBase
    {
        public string Id { get; set; }

        public string Label { get; set; }

        public string MessageType { get; set; }

        public DateTime? TimeSent { get; set; }

        public string FriendlyMessageType => TypeHumanizer.ToName(MessageType);
    }
}