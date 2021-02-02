namespace ServiceInsight.MessageFlow
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using ExtensionMethods;
    using Framework;
    using Mindscape.WpfDiagramming;
    using Models;

    public class SagaInvocation
    {
        public SagaInvocation(SagaInfo saga, StoredMessage triggeringMessage)
        {
            ID = saga.SagaId;
            SagaType = TypeHumanizer.ToName(saga.SagaType);
            IsSagaCompleted = saga.ChangeStatus == "Completed";
            IsSagaInitiated = string.IsNullOrEmpty(triggeringMessage.GetHeaderByKey(MessageHeaderKeys.SagaId)) &&
                              !string.IsNullOrEmpty(triggeringMessage.GetHeaderByKey(MessageHeaderKeys.OriginatedSagaId));
        }

        public Guid ID { get; }
        public string SagaType { get; }
        public bool IsSagaCompleted { get; }
        public bool IsSagaInitiated { get; }
    }

    [DebuggerDisplay("Type={Message.FriendlyMessageType}, Id={Message.Id}")]
    public class MessageNode : DiagramNode
    {
        int BaseNodeHeight = 36;
        int EndpointsPartHeight = 25;

        public MessageNode(MessageFlowViewModel owner, StoredMessage message)
        {
            IsResizable = false;
            Owner = owner;
            Data = message;
            ExceptionMessage = message.GetHeaderByKey(MessageHeaderKeys.ExceptionType);
            SagaInvocations = new List<SagaInvocation>(ProcessSagaInvocations(message));

            BaseNodeHeight += HasSaga ? 10 : 0;
            Bounds = new Rect(0, 0, 100, CalculateNodeHeight());

            CopyConversationIDCommand = owner.CopyConversationIDCommand;
            CopyMessageURICommand = owner.CopyMessageURICommand;
            SearchByMessageIDCommand = owner.SearchByMessageIDCommand;
            RetryMessageCommand = owner.RetryMessageCommand;

            message.ChangedProperty(nameof(StoredMessage.Status)).Subscribe(_ =>
            {
                OnPropertyChanged("HasFailed");
                OnPropertyChanged("HasRetried");
            });
        }

        IEnumerable<SagaInvocation> ProcessSagaInvocations(StoredMessage message)
        {
            if (message.Sagas == null)
            {
                yield break;
            }

            foreach (var saga in message.Sagas)
            {
                if (saga != null)
                {
                    yield return new SagaInvocation(saga, message);
                }
            }
        }

        public StoredMessage Message => Data as StoredMessage;

        public MessageFlowViewModel Owner { get; }

        public ICommand CopyConversationIDCommand { get; }

        public ICommand CopyMessageURICommand { get; }

        public ICommand SearchByMessageIDCommand { get; }

        public ICommand RetryMessageCommand { get; }

        public void ShowBody()
        {
            Owner.ShowMessageBody();
        }

        public void ShowException()
        {
            Owner.ShowException(new ExceptionDetails(Message));
        }

        public bool ShowEndpoints { get; set; }

        public void OnShowEndpointsChanged()
        {
            Bounds = new Rect(default, new Size(Bounds.Width, CalculateNodeHeight()));
        }

        double CalculateNodeHeight()
        {
            return BaseNodeHeight + (SagaInvocations.Count * 20) + (ShowEndpoints ? EndpointsPartHeight : 0);
        }

        public bool ShowExceptionInfo => !string.IsNullOrEmpty(ExceptionMessage);

        public string NSBVersion => Message.GetHeaderByKey(MessageHeaderKeys.Version);

        public bool IsPublished => Message.MessageIntent == MessageIntent.Publish;

        public bool IsEventMessage => IsPublished && !IsTimeout;

        public bool IsCommandMessage => !IsPublished && !IsTimeout;

        public bool IsTimeoutMessage => IsTimeout;

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
                                 Message.Status == MessageStatus.RepeatedFailure ||
                                 Message.Status == MessageStatus.ArchivedFailure;

        public bool HasRetried => Message.Status == MessageStatus.RetryIssued;

        public IList<SagaInvocation> SagaInvocations { get; }

        public bool HasSaga => SagaInvocations.Any();

        public string ExceptionMessage { get; set; }

        public bool IsFocused { get; set; }
    }
}