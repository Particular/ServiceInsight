namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;
    using ServiceInsight.Models;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem, IComparable<Arrow>
    {
        public Arrow(StoredMessage message, IMessageCommandContainer container)
        {
            SelectedMessage = message;

            CopyConversationIDCommand = container?.CopyConversationIDCommand;
            CopyMessageURICommand = container?.CopyMessageURICommand;
            RetryMessageCommand = container?.RetryMessageCommand;
            SearchByMessageIDCommand = container?.SearchByMessageIDCommand;
            ChangeCurrentMessage = container?.ChangeSelectedMessageCommand;
            ShowExceptionCommand = container?.ShowExceptionCommand;
        }

        public ICommand RetryMessageCommand { get; set; }

        public ICommand CopyConversationIDCommand { get; set; }

        public ICommand ShowExceptionCommand { get; set; }

        public ICommand CopyMessageURICommand { get; set; }

        public ICommand SearchByMessageIDCommand { get; set; }

        public ICommand ChangeCurrentMessage { get; set; }

        public StoredMessage SelectedMessage { get; }

        public Handler FromHandler { get; set; }

        public Handler ToHandler { get; set; }

        public MessageProcessingRoute MessageProcessingRoute { get; set; }

        public Direction Direction { get; set; }

        public ArrowType Type { get; set; }

        public Endpoint Receiving => SelectedMessage.ReceivingEndpoint;

        public Endpoint Sending => SelectedMessage.SendingEndpoint;

        public DateTime? SentTime => SelectedMessage.TimeSent;

        public string MessageId => SelectedMessage.MessageId;

        public MessageStatus Status => SelectedMessage.Status;

        public double Width { get; set; }

        protected override void OnIsFocusedChanged()
        {
            base.OnIsFocusedChanged();
            if (Route != null)
            {
                Route.IsFocused = IsFocused;
            }
        }

        public MessageProcessingRoute Route { get; set; }

        public int CompareTo(Arrow other)
        {
            if (!other.SentTime.HasValue && !SentTime.HasValue)
            {
                return 0;
            }

            if (SentTime.HasValue && !other.SentTime.HasValue)
            {
                return 1;
            }

            if (!SentTime.HasValue)
            {
                return -1;
            }

            return SentTime.Value.CompareTo(other.SentTime.Value);
        }
    }
}