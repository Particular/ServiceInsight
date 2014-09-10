namespace Particular.ServiceInsight.Desktop.Models
{
    using System;
    using Caliburn.Micro;
    using Framework;

    [Serializable]
    public class MessageInfo : PropertyChangedBase
    {
        public MessageInfo()
        {
        }

        public MessageInfo(string id, string label, DateTime timeSent)
        {
            Id = id;
            Label = label;
            TimeSent = timeSent;
        }

        public string Id { get; set; }

        public string Label { get; set; }

        public string MessageType { get; set; }

        public DateTime TimeSent { get; set; }

        public string FriendlyMessageType { get { return TypeHumanizer.ToName(MessageType); } }
    }
}