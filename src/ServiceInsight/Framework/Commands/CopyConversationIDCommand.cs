namespace ServiceInsight.Framework.Commands
{
    using Framework;
    using Models;

    public class CopyConversationIDCommand : BaseCommand
    {
        readonly IClipboard clipboard;

        public CopyConversationIDCommand(IClipboard clipboard)
        {
            this.clipboard = clipboard;
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

            clipboard.CopyTo(message.ConversationId);
        }
    }
}