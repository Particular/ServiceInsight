namespace ServiceInsight.MessageFlow
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Framework;
    using Mindscape.WpfDiagramming;
    using Models;
    using Pirac;

    [DebuggerDisplay("Type={Message.FriendlyMessageType}, Id={Message.Id}")]
    public class MessageNode : DiagramNode
    {
        int heightNoEndpoints = 56;
        int endpointsHeight = 25;

        public MessageNode(MessageFlowViewModel owner, StoredMessage message)
        {
            IsResizable = false;
            Owner = owner;
            Data = message;
            ExceptionMessage = message.GetHeaderByKey(MessageHeaderKeys.ExceptionType);
            SagaType = ProcessSagaType(message);

            heightNoEndpoints += HasSaga ? 10 : 0;
            Bounds = new Rect(0, 0, 100, heightNoEndpoints);

            CopyConversationIDCommand = owner.CopyConversationIDCommand;
            CopyMessageURICommand = owner.CopyMessageURICommand;
            SearchByMessageIDCommand = owner.SearchByMessageIDCommand;
            RetryMessageCommand = owner.RetryMessageCommand;
            ShowExceptionCommand = Command.Create(ShowException);

            message.ChangedProperty(nameof(StoredMessage.Status)).Subscribe(_ =>
            {
                OnPropertyChanged("HasFailed");
                OnPropertyChanged("HasRetried");
            });
        }

        string ProcessSagaType(StoredMessage message)
        {
            if (message.Sagas == null)
            {
                return string.Empty;
            }

            var originatingSaga = message.Sagas.FirstOrDefault();
            if (originatingSaga == null)
            {
                return string.Empty;
            }

            return TypeHumanizer.ToName(originatingSaga.SagaType);
        }

        public StoredMessage Message => Data as StoredMessage;

        public MessageFlowViewModel Owner { get; }

        public ICommand CopyConversationIDCommand { get; }

        public ICommand CopyMessageURICommand { get; }

        public ICommand SearchByMessageIDCommand { get; }

        public ICommand RetryMessageCommand { get; }

        public ICommand ShowExceptionCommand { get; }

        public void ShowBody()
        {
            Owner.ShowMessageBody();
        }

        void ShowException()
        {
            Owner.ShowException(new ExceptionDetails(Message));
        }

        public bool ShowEndpoints { get; set; }

        public void OnShowEndpointsChanged()
        {
            Bounds = new Rect(new Point(), new Size(Bounds.Width, heightNoEndpoints + (ShowEndpoints ? endpointsHeight : 0)));
        }

        public bool ShowExceptionInfo => !string.IsNullOrEmpty(ExceptionMessage);

        public string NSBVersion => Message.GetHeaderByKey(MessageHeaderKeys.Version);

        public bool IsPublished => Message.MessageIntent == MessageIntent.Publish;

        public bool IsEventMessage => IsPublished && !IsTimeout;

        public bool IsCommandMessage => !IsPublished && !IsTimeout;

        public bool IsTimeoutMessage => IsTimeout;

        public bool IsSagaInitiated => string.IsNullOrEmpty(Message.GetHeaderByKey(MessageHeaderKeys.SagaId)) && !string.IsNullOrEmpty(Message.GetHeaderByKey(MessageHeaderKeys.OriginatedSagaId));

        public bool IsSagaCompleted
        {
            get
            {
                var status = Message.InvokedSagas == null ? null : Message.InvokedSagas.FirstOrDefault();
                return status != null && status.ChangeStatus == "Completed";
            }
        }

        public bool IsTimeout
        {
            get
            {
                var isTimeoutString = Message.GetHeaderByKey(MessageHeaderKeys.IsSagaTimeout);
                return !string.IsNullOrEmpty(isTimeoutString) && bool.Parse(isTimeoutString);
            }
        }

        public DateTime? TimeSent
        {
            get
            {
                var timeString = Message.GetHeaderByKey(MessageHeaderKeys.TimeSent);
                if (string.IsNullOrEmpty(timeString))
                {
                    return null;
                }

                return DateTime.ParseExact(timeString, HeaderInfo.MessageDateFormat, System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public bool HasFailed => Message.Status == MessageStatus.Failed ||
       Message.Status == MessageStatus.RepeatedFailure || Message.Status == MessageStatus.ArchivedFailure;

        public bool HasRetried => Message.Status == MessageStatus.RetryIssued;

        public string SagaType { get; }

        public bool HasSaga => !string.IsNullOrEmpty(SagaType);

        public string ExceptionMessage { get; set; }

        public bool IsFocused { get; set; }
    }
}