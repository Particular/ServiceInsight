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
            return parameter is StoredMessage;
        }

        public override void Execute(object parameter)
        {
            if (!(parameter is StoredMessage message))
            {
                return;
            }

            clipboard.CopyTo(message.ConversationId);
        }
    }
}