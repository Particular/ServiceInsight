namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;
    using Particular.ServiceInsight.Desktop.Models;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem, IComparable<Arrow>
    {
        readonly string conversationId;
        readonly string id;
        readonly string messageId;
        readonly MessageStatus status;

        StoredMessage storedMessage;
        List<Header> headers;
        DateTime? timesent;

        public Arrow(string messageId, string conversationId, MessageStatus status, string id, DateTime? timesent, List<Header> headers, IMessageCommandContainer container)
        {
            this.messageId = messageId;
            this.conversationId = conversationId;
            this.status = status;
            this.id = id;
            this.timesent = timesent;
            this.headers = headers;

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

        public Endpoint Receiving { get; set; }

        public Endpoint Sending { get; set; }

        public StoredMessage SelectedMessage
        {
            get
            {
                return storedMessage = storedMessage ?? new StoredMessage
                {
                    ConversationId = conversationId,
                    ReceivingEndpoint = Receiving,
                    MessageId = messageId,
                    TimeSent = timesent,
                    Id = id,
                    Status = status,
                    SendingEndpoint = Sending,
                    Headers = headers?.Select(h => new StoredMessageHeader { Key = h.key, Value = h.value }).ToList()
                };
            }
        }

        public Handler FromHandler { get; set; }

        public Handler ToHandler { get; set; }

        public MessageProcessingRoute MessageProcessingRoute { get; set; }

        public Direction Direction { get; set; }

        public ArrowType Type { get; set; }

        public DateTime? SentTime { get; set; }

        public string MessageId
        {
            get { return messageId; }
        }

        public MessageStatus Status
        {
            get { return status; }
        }

        public double Width { get; set; }

        public override DiagramItem GetFocusableItem()
        {
            return this;
        }

        public override void OnIsFocusedChanged()
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