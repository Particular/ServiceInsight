using System.Diagnostics;
using System.Windows;
using Mindscape.WpfDiagramming;
using NServiceBus.Profiler.Desktop.Models;

namespace NServiceBus.Profiler.Desktop.MessageFlow
{
    [DebuggerDisplay("Type={Message.FriendlyMessageType}, Id={Message.Id}")]
    public class MessageNode : DiagramNode
    {
        public MessageNode(IMessageFlowViewModel owner, StoredMessage message)
        {
            Owner = owner;
            Bounds = new Rect(100, 100, 203, 40);
            ZOrder = 1;
            Data = message;
            IsResizable = false;
            ExceptionMessage = message.GetHeaderByKey(MessageHeaderKeys.ExceptionType);
        }

        public StoredMessage Message
        {
            get { return Data as StoredMessage; }
        }

        public IMessageFlowViewModel Owner
        {
            get; private set;
        }

        public void CopyConversationId()
        {
            Owner.CopyConversationId(Message);
        }

        public void CopyHeaders()
        {
            Owner.CopyMessageHeaders(Message);
        }

        public async void Retry()
        {
            await Owner.RetryMessage(Message);
            Message.Status = MessageStatus.RetryIssued;
            base.OnPropertyChanged("HasFailed");
        }

        public bool CanRetry()
        {
            return HasFailed;

        }

        public void ShowBody()
        {
            Owner.ShowMessageBody(Message);
        }

        public void Refresh()
        {
        }

        public bool ShowEndpoints
        {
            get; set;
        }

        public void OnShowEndpointsChanged()
        {
            Bounds = new Rect(Bounds.Location, new Size(Bounds.Width, ShowEndpoints ? 75 : 40));
        }

        public bool ShowExceptionInfo
        {
            get { return !string.IsNullOrEmpty(ExceptionMessage); }
        }

        public string NSBVersion
        {
            get { return Message.GetHeaderByKey(MessageHeaderKeys.Version); }
        }

        public string SecondLevelRetries
        {
            get { return Message.GetHeaderByKey(MessageHeaderKeys.Retries); }
        }

        public bool IsPublished
        {
            get { return Message.MessageIntent == MessageIntent.Publish; }
        }

        public bool HasFailed
        {
            get
            {
                return Message.Status == MessageStatus.Failed ||
                       Message.Status == MessageStatus.RepeatedFailure;
            }
        }

        public string ExceptionMessage
        {
            get; set;
        }

        public bool IsCurrentMessage { get; set; }
    }
}