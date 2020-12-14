using ServiceInsight.MessageList;

namespace ServiceInsight.Framework.Commands
{
    using Framework;
    using Models;

    public class CopyMessageURICommand : BaseCommand
    {
        readonly IClipboard clipboard;
        readonly MessageListViewModel parent;

        public CopyMessageURICommand(IClipboard clipboard, MessageListViewModel parent)
        {
            this.clipboard = clipboard;
            this.parent = parent;
        }

        public override bool CanExecute(object parameter)
        {
            var message = parameter as StoredMessage;
            return message != null;
        }

        public override void Execute(object parameter)
        {
            var message = parameter as StoredMessage;
            if (message == null)
            {
                return;
            }

            if (parent.ServiceControl != null)
            {
                var uri = parent.ServiceControl.CreateServiceInsightUri(message).ToString();
                clipboard.CopyTo(uri);
            }
        }
    }
}