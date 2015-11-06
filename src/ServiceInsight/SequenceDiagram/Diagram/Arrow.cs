namespace ServiceInsight.SequenceDiagram.Diagram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;
    using Autofac;
    using Particular.ServiceInsight.Desktop.Framework.Commands;
    using Particular.ServiceInsight.Desktop.Framework.Settings;
    using Particular.ServiceInsight.Desktop.Framework.UI.ScreenManager;
    using Particular.ServiceInsight.Desktop.MessageFlow;
    using Particular.ServiceInsight.Desktop.Models;
    using ReactiveUI;

    [DebuggerDisplay("{Type}->{Name}")]
    public class Arrow : DiagramItem, IComparable<Arrow>
    {
        readonly string conversationId;
        readonly string id;
        readonly string messageId;
        readonly MessageStatus status;

        StoredMessage storedMessage;
        List<Header> headers;
        IContainer container;
        DateTime? timesent;

        public Arrow(string messageId, string conversationId, MessageStatus status, string id, DateTime? timesent, List<Header> headers, IContainer container)
        {
            this.messageId = messageId;
            this.conversationId = conversationId;
            this.status = status;
            this.id = id;
            this.timesent = timesent;
            this.headers = headers;
            this.container = container;

            CopyConversationIDCommand = container.Resolve<CopyConversationIDCommand>();
            CopyMessageURICommand = container.Resolve<CopyMessageURICommand>();
            RetryMessageCommand = container.Resolve<RetryMessageCommand>();
            SearchByMessageIDCommand = container.Resolve<SearchByMessageIDCommand>();
            ChangeCurrentMessage = container.Resolve<ChangeSelectedMessageCommand>();

            var cmd = new ReactiveCommand();
            cmd.Subscribe(_ => ShowException());
            DisplayExceptionDetailsCommand = cmd;
        }

        void ShowException()
        {
            var windowManager = container.Resolve<IWindowManagerEx>();
            var settingsProvider = container.Resolve<ISettingsProvider>();
            var model = new ExceptionDetailViewModel(settingsProvider, new ExceptionDetails(SelectedMessage));

            windowManager.ShowDialog(model);
        }

        public ICommand RetryMessageCommand { get; private set; }
        public ICommand CopyConversationIDCommand { get; private set; }
        public ICommand CopyMessageURICommand { get; private set; }
        public ICommand SearchByMessageIDCommand { get; private set; }
        public ICommand DisplayExceptionDetailsCommand { get; private set; }
        public ICommand ChangeCurrentMessage { get; private set; }

        public Endpoint receiving { get; set; }
        public Endpoint sending { get; set; }

        public StoredMessage SelectedMessage
        {
            get
            {
                return storedMessage = storedMessage ?? new StoredMessage
                {
                    ConversationId = conversationId,
                    ReceivingEndpoint = receiving,
                    MessageId = messageId,
                    TimeSent = timesent,
                    Id = id,
                    Status = status,
                    SendingEndpoint = sending,
                    Headers = headers.Select(h => new StoredMessageHeader { Key = h.key, Value = h.value }).ToList()
                };
            }
        }

        public Handler FromHandler { get; set; }

        public Handler ToHandler { get; set; }

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
        public bool IsFocused { get; set; }

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