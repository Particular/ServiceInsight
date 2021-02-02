namespace ServiceInsight.Framework.Commands
{
    using ServiceInsight.Framework;
    using ServiceInsight.MessageList;
    using ServiceInsight.Models;

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
            return parameter is StoredMessage;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is StoredMessage message))
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